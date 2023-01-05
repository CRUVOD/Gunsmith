using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[ExecuteAlways]
[AddComponentMenu("")]
[FeedbackPath("Audio/Sound")]
[FeedbackHelp("This feedback lets you play the specified AudioClip, either via event (you'll need something in your scene to catch a MMSfxEvent, for example a MMSoundManager), or cached (AudioSource gets created on init, and is then ready to be played), or on demand (instantiated on Play). For all these methods you can define a random volume between min/max boundaries (just set the same value in both fields if you don't want randomness), random pitch, and an optional AudioMixerGroup.")]
public class FeedbackSound : Feedback
{
    /// <summary>
    /// The possible methods to play the sound with. 
    /// Event : sends a MMSfxEvent, you'll need a class to catch this event and play the sound
    /// Cached : creates and stores an audiosource to play the sound with, parented to the owner
    /// OnDemand : creates an audiosource and destroys it everytime you want to play the sound
    /// </summary>
    public enum PlayMethods { Event, Cached, OnDemand, Pool }

    [Header("Sound")]
    /// the sound clip to play
    [Tooltip("the sound clip to play")]
    public AudioClip Sfx;

    [Header("Random Sound")]
    /// an array to pick a random sfx from
    [Tooltip("an array to pick a random sfx from")]
    public AudioClip[] RandomSfx;

    [Header("Method")]
    /// the play method to use when playing the sound (event, cached or on demand)
    [Tooltip("the play method to use when playing the sound (event, cached or on demand)")]
    public PlayMethods PlayMethod = PlayMethods.Event;
    /// the size of the pool when in Pool mode
    [Tooltip("the size of the pool when in Pool mode")]
    [MMFEnumCondition("PlayMethod", (int)PlayMethods.Pool)]
    public int PoolSize = 10;

    [Header("Volume")]
    /// the minimum volume to play the sound at
    [Tooltip("the minimum volume to play the sound at")]
    public float MinVolume = 1f;
    /// the maximum volume to play the sound at
    [Tooltip("the maximum volume to play the sound at")]
    public float MaxVolume = 1f;

    [Header("Pitch")]
    /// the minimum pitch to play the sound at
    [Tooltip("the minimum pitch to play the sound at")]
    public float MinPitch = 1f;
    /// the maximum pitch to play the sound at
    [Tooltip("the maximum pitch to play the sound at")]
    public float MaxPitch = 1f;

    [Header("Mixer")]
    /// the audiomixer to play the sound with (optional)
    [Tooltip("the audiomixer to play the sound with (optional)")]
    public AudioMixerGroup SfxAudioMixerGroup;

    /// the duration of this feedback is the duration of the clip being played
    public override float FeedbackDuration { get { return GetDuration(); } }

    protected AudioClip _randomClip;
    protected AudioSource _cachedAudioSource;
    protected AudioSource[] _pool;
    protected AudioSource tempAudioSource;
    protected float _duration;
    protected AudioSource _editorAudioSource;
    GameObject temporaryAudioHost;

    /// <summary>
    /// Custom init to cache the audiosource if required
    /// </summary>
    /// <param name="owner"></param>
    protected override void CustomInitialisation(GameObject owner)
    {
        base.CustomInitialisation(owner);
        if (PlayMethod == PlayMethods.Cached)
        {
            _cachedAudioSource = CreateAudioSource(owner, "CachedFeedbackAudioSource");
        }
        if (PlayMethod == PlayMethods.Pool)
        {
            // create a pool
            _pool = new AudioSource[PoolSize];
            for (int i = 0; i < PoolSize; i++)
            {
                _pool[i] = CreateAudioSource(owner, "PooledAudioSource" + i);
            }
        }
    }

    protected virtual AudioSource CreateAudioSource(GameObject owner, string audioSourceName)
    {
        // we create a temporary game object to host our audio source
        temporaryAudioHost = new GameObject(audioSourceName);
        SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, this.gameObject.scene);
        // we set the temp audio's position
        temporaryAudioHost.transform.position = owner.transform.position;
        temporaryAudioHost.transform.SetParent(owner.transform);
        // we add an audio source to that host
        tempAudioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
        tempAudioSource.playOnAwake = false;
        return tempAudioSource;
    }

    /// <summary>
    /// Plays either a random sound or the specified sfx
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (!Active)
        {
            return;
        }

        float intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;

        if (Sfx != null)
        {
            _duration = Sfx.length;
            PlaySound(Sfx, position, intensityMultiplier);
            return;
        }

        if (RandomSfx.Length > 0)
        {
            _randomClip = RandomSfx[Random.Range(0, RandomSfx.Length)];

            if (_randomClip != null)
            {
                _duration = _randomClip.length;
                PlaySound(_randomClip, position, intensityMultiplier);
            }

        }
    }

    protected virtual float GetDuration()
    {
        if (Sfx != null)
        {
            return Sfx.length;
        }

        float longest = 0f;
        if ((RandomSfx != null) && (RandomSfx.Length > 0))
        {
            foreach (AudioClip clip in RandomSfx)
            {
                if ((clip != null) && (clip.length > longest))
                {
                    longest = clip.length;
                }
            }

            return longest;
        }

        return 0f;
    }

    /// <summary>
    /// Plays a sound differently based on the selected play method
    /// </summary>
    /// <param name="sfx"></param>
    /// <param name="position"></param>
    protected virtual void PlaySound(AudioClip sfx, Vector3 position, float intensity)
    {
        float volume = Random.Range(MinVolume, MaxVolume);

        if (!Timing.ConstantIntensity)
        {
            volume = volume * intensity;
        }

        float pitch = Random.Range(MinPitch, MaxPitch);

        int timeSamples = 0;

        if (PlayMethod == PlayMethods.Event)
        {
            tempAudioSource = SfxEvent.Trigger(sfx, SfxAudioMixerGroup, volume, pitch);
            temporaryAudioHost = tempAudioSource.gameObject;
            return;
        }

        if (PlayMethod == PlayMethods.OnDemand)
        {
            // we create a temporary game object to host our audio source
            temporaryAudioHost = new GameObject("TempAudio");
            SceneManager.MoveGameObjectToScene(temporaryAudioHost.gameObject, this.gameObject.scene);
            // we set the temp audio's position
            temporaryAudioHost.transform.position = position;
            // we add an audio source to that host
            AudioSource audioSource = temporaryAudioHost.AddComponent<AudioSource>() as AudioSource;
            PlayAudioSource(audioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup);
            // we destroy the host after the clip has played
            Destroy(temporaryAudioHost, sfx.length);
        }

        if (PlayMethod == PlayMethods.Cached)
        {
            // we set that audio source clip to the one in paramaters
            PlayAudioSource(_cachedAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup);
        }

        if (PlayMethod == PlayMethods.Pool)
        {
            tempAudioSource = GetAudioSourceFromPool();
            if (tempAudioSource != null)
            {
                PlayAudioSource(tempAudioSource, sfx, volume, pitch, timeSamples, SfxAudioMixerGroup);
            }
        }
    }

    protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        if (PlayMethod == PlayMethods.Event && (tempAudioSource != null))
        {
            AudioManagerSoundControlEvent.Trigger(AudioManagerSoundControlEventTypes.Free, 0, tempAudioSource);
        }
        else if ((PlayMethod == PlayMethods.OnDemand) && (temporaryAudioHost != null))
        {
            Destroy(temporaryAudioHost);
        }
        StopAllCoroutines();
    }

    /// <summary>
    /// Plays the audio source with the specified volume and pitch
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="sfx"></param>
    /// <param name="volume"></param>
    /// <param name="pitch"></param>
    protected virtual void PlayAudioSource(AudioSource audioSource, AudioClip sfx, float volume, float pitch, int timeSamples, AudioMixerGroup audioMixerGroup = null)
    {
        // we set that audio source clip to the one in paramaters
        audioSource.clip = sfx;
        audioSource.timeSamples = timeSamples;
        // we set the audio source volume to the one in parameters
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        // we set our loop setting
        audioSource.loop = false;
        if (audioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }
        // we start playing the sound
        audioSource.Play();
    }

    /// <summary>
    /// Gets an audio source from the pool if possible
    /// </summary>
    /// <returns></returns>
    protected virtual AudioSource GetAudioSourceFromPool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            if (!_pool[i].isPlaying)
            {
                return _pool[i];
            }
        }
        return null;
    }
}
