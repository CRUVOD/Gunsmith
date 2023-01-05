using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEvents
{

}

public enum AudioManagerSoundControlEventTypes
{
    Pause,
    Resume,
    Stop,
    Free
}

/// <summary>
/// An event used to control a specific sound on the AudioManager.
/// You can either search for it by ID, or directly pass an audiosource if you have it.
///
/// Example : AudioManagerSoundControlEvent.Trigger(AudioManagerSoundControlEventTypes.Stop, 33);
/// will cause the sound(s) with an ID of 33 to stop playing
/// </summary>
public struct AudioManagerSoundControlEvent
{
    /// the ID of the sound to control (has to match the one used to play it)
    public int SoundID;
    /// the control mode
    public AudioManagerSoundControlEventTypes AudioControlEventType;
    /// the audiosource to control (if specified)
    public AudioSource TargetSource;

    public AudioManagerSoundControlEvent(AudioManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
    {
        SoundID = soundID;
        TargetSource = source;
        AudioControlEventType = eventType;
    }

    static AudioManagerSoundControlEvent e;
    public static void Trigger(AudioManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
    {
        e.SoundID = soundID;
        e.TargetSource = source;
        e.AudioControlEventType = eventType;
        ExtendedEventManager.TriggerEvent(e);
    }
}

/// <summary>
/// A struct used to trigger sounds, no looping
/// </summary>
public struct SfxEvent
{
    public delegate AudioSource Delegate(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f);
    static private event Delegate OnEvent;

    static public void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    static public void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    static public AudioSource Trigger(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f)
    {
        return OnEvent?.Invoke(clipToPlay, audioGroup, volume, pitch);
    }
}

/// <summary>
/// This event will let you play a sound on the AudioManager with full options and control
///
/// Example : AudioManagerPlaySoundEvent.Trigger(ExplosionSfx, MMSoundManager.MMSoundManagerTracks.Sfx, this.transform.position);
/// will play a clip (here ours is called ExplosionSfx) on the SFX track, at the position of the object calling it
/// </summary>
public struct AudioManagerPlaySoundEvent
{
    public delegate AudioSource Delegate(AudioClip clip, AudioManagerOptions options);
    static private event Delegate OnEvent;

    static public void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    static public void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    static public AudioSource Trigger(AudioClip clip, AudioManagerOptions options)
    {
        return OnEvent?.Invoke(clip, options);
    }

    static public AudioSource Trigger(AudioClip audioClip, AudioManager.AudioManagerTracks AudioManagerTrack, Vector3 location,
        bool loop = false, float volume = 1.0f, int ID = 0,
        bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, TweenType fadeTween = null,
        bool persistent = false,
        AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
        float pitch = 1f, float panStereo = 0f, float spatialBlend = 0.0f,
        bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,
        bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false, int priority = 128, float reverbZoneMix = 1f,
        float dopplerLevel = 1f, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic, float minDistance = 1f, float maxDistance = 500f)
    {
        AudioManagerOptions options = AudioManagerOptions.Default;
        options.AudioManagerTrack = AudioManagerTrack;
        options.Location = location;
        options.Loop = loop;
        options.Volume = volume;
        options.ID = ID;
        options.Fade = fade;
        options.FadeInitialVolume = fadeInitialVolume;
        options.FadeDuration = fadeDuration;
        options.FadeTween = fadeTween;
        options.Persistent = persistent;
        options.RecycleAudioSource = recycleAudioSource;
        options.AudioGroup = audioGroup;
        options.Pitch = pitch;
        options.PanStereo = panStereo;
        options.SpatialBlend = spatialBlend;
        options.SoloSingleTrack = soloSingleTrack;
        options.SoloAllTracks = soloAllTracks;
        options.AutoUnSoloOnEnd = autoUnSoloOnEnd;
        options.BypassEffects = bypassEffects;
        options.BypassListenerEffects = bypassListenerEffects;
        options.BypassReverbZones = bypassReverbZones;
        options.Priority = priority;
        options.ReverbZoneMix = reverbZoneMix;
        options.DopplerLevel = dopplerLevel;
        options.Spread = spread;
        options.RolloffMode = rolloffMode;
        options.MinDistance = minDistance;
        options.MaxDistance = maxDistance;

        return OnEvent?.Invoke(audioClip, options);
    }
}
