using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour, ExtendedEventListener<AudioManagerSoundControlEvent>
{
    // Manages and listens for audio events, a singleton
    public static AudioManager instance;

    /// the possible ways to manage a track
    public enum AudioManagerTracks { Sfx, Music, UI, Master, Other }

    [Header("AudioMixerGroups")]
    public AudioMixerGroup sfxMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup masterMixerGroup;

    [Header("Pool")]
    /// the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once 
    [Tooltip("the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once")]
    public int AudioSourcePoolSize = 10;
    /// whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.
    [Tooltip("whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.")]
    public bool PoolCanExpand = true;

    protected AudioPool audioPool;
    protected GameObject tempAudioSourceGameObject;
    protected AudioManagerSound sound;
    protected List<AudioManagerSound> sounds;
    protected AudioSource tempAudioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
            return;
        }

        InitialiseAudioManager();
    }

    private void Start()
    {
        instance = this;
    }

    /// <summary>
    /// Initializes the pool, fills it, registers to the scene loaded event
    /// </summary>
    protected virtual void InitialiseAudioManager()
    {
        if (audioPool == null)
        {
            audioPool = new AudioPool();
        }
        sounds = new List<AudioManagerSound>();
        audioPool.FillAudioSourcePool(AudioSourcePoolSize, this.transform);

    }

    #region Find

    /// <summary>
    /// Returns an audio source played with the specified ID, if one is found
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public virtual AudioSource FindByID(int ID)
    {
        foreach (AudioManagerSound sound in sounds)
        {
            if (sound.ID == ID)
            {
                return sound.Source;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns an audio source played with the specified ID, if one is found
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public virtual AudioSource FindByClip(AudioClip clip)
    {
        foreach (AudioManagerSound sound in sounds)
        {
            if (sound.Source.clip == clip)
            {
                return sound.Source;
            }
        }

        return null;
    }

    #endregion

    #region PlaySound

    /// <summary>
    /// Plays a sound, separate options object signature
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public virtual AudioSource PlaySound(AudioClip audioClip, AudioManagerOptions options)
    {
        return PlaySound(audioClip, options.AudioManagerTrack, options.Location,
            options.Loop, options.Volume, options.ID,
            options.Fade, options.FadeInitialVolume, options.FadeDuration, options.FadeTween,
            options.Persistent,
            options.RecycleAudioSource, options.AudioGroup,
            options.Pitch, options.PanStereo, options.SpatialBlend,
            options.SoloSingleTrack, options.SoloAllTracks, options.AutoUnSoloOnEnd,
            options.BypassEffects, options.BypassListenerEffects, options.BypassReverbZones, options.Priority,
            options.ReverbZoneMix,
            options.DopplerLevel, options.Spread, options.RolloffMode, options.MinDistance, options.MaxDistance,
            options.DoNotAutoRecycleIfNotDonePlaying, options.PlaybackTime
        );
    }

    /// <summary>
    /// Plays a sound, signature with all options
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="AudioManagerTrack"></param>
    /// <param name="location"></param>
    /// <param name="loop"></param>
    /// <param name="volume"></param>
    /// <param name="ID"></param>
    /// <param name="fade"></param>
    /// <param name="fadeInitialVolume"></param>
    /// <param name="fadeDuration"></param>
    /// <param name="fadeTween"></param>
    /// <param name="persistent"></param>
    /// <param name="recycleAudioSource"></param>
    /// <param name="audioGroup"></param>
    /// <param name="pitch"></param>
    /// <param name="panStereo"></param>
    /// <param name="spatialBlend"></param>
    /// <param name="soloSingleTrack"></param>
    /// <param name="soloAllTracks"></param>
    /// <param name="autoUnSoloOnEnd"></param>
    /// <param name="bypassEffects"></param>
    /// <param name="bypassListenerEffects"></param>
    /// <param name="bypassReverbZones"></param>
    /// <param name="priority"></param>
    /// <param name="reverbZoneMix"></param>
    /// <param name="dopplerLevel"></param>
    /// <param name="spread"></param>
    /// <param name="rolloffMode"></param>
    /// <param name="minDistance"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    public virtual AudioSource PlaySound(AudioClip audioClip, AudioManagerTracks AudioManagerTrack, Vector3 location,
        bool loop = false, float volume = 1.0f, int ID = 0,
        bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, TweenType fadeTween = null,
        bool persistent = false,
        AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
        float pitch = 1f, float panStereo = 0f, float spatialBlend = 0.0f,
        bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,
        bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false, int priority = 128, float reverbZoneMix = 1f,
        float dopplerLevel = 1f, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic, float minDistance = 1f, float maxDistance = 500f,
        bool doNotAutoRecycleIfNotDonePlaying = false, float playbackTime = 0f
    )
    {
        if (this == null) { return null; }
        if (!audioClip) { return null; }

        // audio source setup ---------------------------------------------------------------------------------

        // we reuse an audiosource if one is passed in parameters
        AudioSource audioSource = recycleAudioSource;

        if (audioSource == null)
        {
            // we pick an idle audio source from the pool if possible
            audioSource = audioPool.GetAvailableAudioSource(PoolCanExpand, this.transform);
            if ((audioSource != null) && (!loop))
            {
                recycleAudioSource = audioSource;
                // we destroy the host after the clip has played (if it not tag for reusability.
                StartCoroutine(audioPool.AutoDisableAudioSource(audioClip.length / Mathf.Abs(pitch), audioSource, audioClip, doNotAutoRecycleIfNotDonePlaying));
            }
        }

        // we create an audio source if needed
        if (audioSource == null)
        {
            tempAudioSourceGameObject = new GameObject("Audio_" + audioClip.name);
            SceneManager.MoveGameObjectToScene(tempAudioSourceGameObject, this.gameObject.scene);
            audioSource = tempAudioSourceGameObject.AddComponent<AudioSource>();
        }

        // audio source settings ---------------------------------------------------------------------------------

        audioSource.transform.position = location;
        audioSource.clip = audioClip;
        audioSource.pitch = pitch;
        audioSource.spatialBlend = spatialBlend;
        audioSource.panStereo = panStereo;
        audioSource.loop = loop;
        audioSource.bypassEffects = bypassEffects;
        audioSource.bypassListenerEffects = bypassListenerEffects;
        audioSource.bypassReverbZones = bypassReverbZones;
        audioSource.priority = priority;
        audioSource.reverbZoneMix = reverbZoneMix;
        audioSource.dopplerLevel = dopplerLevel;
        audioSource.spread = spread;
        audioSource.rolloffMode = rolloffMode;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.time = playbackTime;
        audioSource.volume = volume;

        // track and volume ---------------------------------------------------------------------------------

        //if (settingsSo != null)
        //{
        //    audioSource.outputAudioMixerGroup = settingsSo.MasterAudioMixerGroup;
        //    switch (AudioManagerTrack)
        //    {
        //        case AudioManagerTracks.Master:
        //            audioSource.outputAudioMixerGroup = settingsSo.MasterAudioMixerGroup;
        //            break;
        //        case AudioManagerTracks.Music:
        //            audioSource.outputAudioMixerGroup = settingsSo.MusicAudioMixerGroup;
        //            break;
        //        case AudioManagerTracks.Sfx:
        //            audioSource.outputAudioMixerGroup = settingsSo.SfxAudioMixerGroup;
        //            break;
        //        case AudioManagerTracks.UI:
        //            audioSource.outputAudioMixerGroup = settingsSo.UIAudioMixerGroup;
        //            break;
        //    }
        //}
        if (audioGroup) { audioSource.outputAudioMixerGroup = audioGroup; }
        switch (AudioManagerTrack)
        {
            case AudioManagerTracks.Master:
                audioSource.outputAudioMixerGroup = masterMixerGroup;
                break;
            case AudioManagerTracks.Music:
                audioSource.outputAudioMixerGroup = musicMixerGroup;
                break;
            case AudioManagerTracks.Sfx:
                audioSource.outputAudioMixerGroup = sfxMixerGroup;
                break;
        }

        // we start playing the sound
        audioSource.Play();
        // we destroy the host after the clip has played if it was a one time AS.
        if (!loop && !recycleAudioSource)
        {
            Destroy(tempAudioSourceGameObject, audioClip.length);
        }

        // we fade the sound in if needed
        if (fade)
        {
            FadeSound(audioSource, fadeDuration, fadeInitialVolume, volume, fadeTween);
        }

        //// we handle soloing
        //if (soloSingleTrack)
        //{
        //    MuteSoundsOnTrack(AudioManagerTrack, true, 0f);
        //    audioSource.mute = false;
        //    if (autoUnSoloOnEnd)
        //    {
        //        MuteSoundsOnTrack(AudioManagerTrack, false, audioClip.length);
        //    }
        //}
        //else if (soloAllTracks)
        //{
        //    MuteAllSounds();
        //    audioSource.mute = false;
        //    if (autoUnSoloOnEnd)
        //    {
        //        StartCoroutine(MuteAllSoundsCoroutine(audioClip.length - playbackTime, false));
        //    }
        //}

        // we prepare for storage
        sound.ID = ID;
        sound.Track = AudioManagerTrack;
        sound.Source = audioSource;
        sound.Persistent = persistent;

        // we check if that audiosource is already being tracked in sounds
        bool alreadyIn = false;
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].Source == audioSource)
            {
                sounds[i] = sound;
                alreadyIn = true;
            }
        }

        if (!alreadyIn)
        {
            sounds.Add(sound);
        }

        // we return the audiosource reference
        return audioSource;
    }

    #endregion

    #region SoundControls

    /// <summary>
    /// Pauses the specified audiosource
    /// </summary>
    /// <param name="source"></param>
    public virtual void PauseSound(AudioSource source)
    {
        source.Pause();
    }

    /// <summary>
    /// resumes play on the specified audio source
    /// </summary>
    /// <param name="source"></param>
    public virtual void ResumeSound(AudioSource source)
    {
        source.Play();
    }

    /// <summary>
    /// Stops the specified audio source
    /// </summary>
    /// <param name="source"></param>
    public virtual void StopSound(AudioSource source)
    {
        source.Stop();
    }

    /// <summary>
    /// Frees a specific sound, stopping it and returning it to the pool
    /// </summary>
    /// <param name="source"></param>
    public virtual void FreeSound(AudioSource source)
    {
        source.Stop();
        if (!audioPool.FreeSound(source))
        {
            Destroy(source.gameObject);
        }
    }

    #endregion

    #region Fades

    /// <summary>
    /// Fades an entire track over the specified duration towards the desired finalVolume
    /// </summary>
    /// <param name="track"></param>
    /// <param name="duration"></param>
    /// <param name="initialVolume"></param>
    /// <param name="finalVolume"></param>
    /// <param name="tweenType"></param>
    public virtual void FadeTrack(AudioManagerTracks track, float duration, float initialVolume = 0f, float finalVolume = 1f, TweenType tweenType = null)
    {
        StartCoroutine(FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tweenType));
    }

    /// <summary>
    /// Fades a target sound towards a final volume over time
    /// </summary>
    /// <param name="source"></param>
    /// <param name="duration"></param>
    /// <param name="initialVolume"></param>
    /// <param name="finalVolume"></param>
    /// <param name="tweenType"></param>
    public virtual void FadeSound(AudioSource source, float duration, float initialVolume, float finalVolume, TweenType tweenType)
    {
        StartCoroutine(FadeCoroutine(source, duration, initialVolume, finalVolume, tweenType));
    }

    /// <summary>
    /// Fades an entire track over time
    /// </summary>
    /// <param name="track"></param>
    /// <param name="duration"></param>
    /// <param name="initialVolume"></param>
    /// <param name="finalVolume"></param>
    /// <param name="tweenType"></param>
    /// <returns></returns>
    protected virtual IEnumerator FadeTrackCoroutine(AudioManagerTracks track, float duration, float initialVolume, float finalVolume, TweenType tweenType)
    {
        float startedAt = Time.unscaledTime;
        if (tweenType == null)
        {
            tweenType = new TweenType(Tweens.TweenCurve.EaseInOutQuartic);
        }
        while (Time.unscaledTime - startedAt <= duration)
        {
            float elapsedTime = Time.unscaledTime - startedAt;
            float newVolume = Tweens.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
            //settingsSo.SetTrackVolume(track, newVolume);
            yield return null;
        }
        //settingsSo.SetTrackVolume(track, finalVolume);
    }

    /// <summary>
    /// Fades an audiosource's volume over time
    /// </summary>
    /// <param name="source"></param>
    /// <param name="duration"></param>
    /// <param name="initialVolume"></param>
    /// <param name="finalVolume"></param>
    /// <param name="tweenType"></param>
    /// <returns></returns>
    protected virtual IEnumerator FadeCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume,TweenType tweenType)
    {
        float startedAt = Time.unscaledTime;
        if (tweenType == null)
        {
            tweenType = new TweenType(Tweens.TweenCurve.EaseInOutQuartic);
        }
        while (Time.unscaledTime - startedAt <= duration)
        {
            float elapsedTime = Time.unscaledTime - startedAt;
            float newVolume = Tweens.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
            source.volume = newVolume;
            yield return null;
        }
        source.volume = finalVolume;
    }

    #endregion

    #region Events

    public virtual void OnExtendedEvent(AudioManagerSoundControlEvent audioControlEvent)
    {
        if (audioControlEvent.TargetSource == null)
        {
            tempAudioSource = FindByID(audioControlEvent.SoundID);
        }
        else
        {
            tempAudioSource = audioControlEvent.TargetSource;
        }

        if (tempAudioSource != null)
        {
            switch (audioControlEvent.AudioControlEventType)
            {
                case AudioManagerSoundControlEventTypes.Pause:
                    PauseSound(tempAudioSource);
                    break;
                case AudioManagerSoundControlEventTypes.Resume:
                    ResumeSound(tempAudioSource);
                    break;
                case AudioManagerSoundControlEventTypes.Stop:
                    StopSound(tempAudioSource);
                    break;
                case AudioManagerSoundControlEventTypes.Free:
                    FreeSound(tempAudioSource);
                    break;
            }
        }
    }

    public virtual AudioSource OnSfxEvent(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f)
    {
        AudioManagerOptions options = AudioManagerOptions.Default;
        options.Location = transform.position;
        options.AudioGroup = audioGroup;
        options.Volume = volume;
        options.Pitch = pitch;
        options.AudioManagerTrack = AudioManagerTracks.Sfx;
        options.Loop = false;

        return PlaySound(clipToPlay, options);
    }

    public virtual AudioSource OnAudioManagerPlaySoundEvent(AudioClip clip, AudioManagerOptions options)
    {
        return PlaySound(clip, options);
    }

    // Start listening for audio events if gameobject/manager is enabled
    protected virtual void OnEnable()
    {
        SfxEvent.Register(OnSfxEvent);
        AudioManagerPlaySoundEvent.Register(OnAudioManagerPlaySoundEvent);
        this.ExtendedEventStartListening<AudioManagerSoundControlEvent>();
    }

    #endregion

    private void OnDestroy()
    {
        SfxEvent.Unregister(OnSfxEvent);
        AudioManagerPlaySoundEvent.Unregister(OnAudioManagerPlaySoundEvent);
        Debug.Log(StackTraceUtility.ExtractStackTrace());
    }
}