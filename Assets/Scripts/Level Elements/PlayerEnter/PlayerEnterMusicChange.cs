using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnterMusicChange : MonoBehaviour
{
    public AudioClip music;

    [Tooltip("Turn the selected music on or off on player enter")]
    public bool musicOn;

    [Header("Music On")]
    public float volume;

    [Header("Music Off")]
    public float fadeDuration;


    private bool hasBeenTriggered;
    private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !hasBeenTriggered)
        {
            player = collision.gameObject.GetComponent<Player>();
            hasBeenTriggered = true;
            if (musicOn)
            {
                PlayMusic();
            }
            else
            {
                StopMusic();
            }
        }
    }

    /// <summary>
    /// Plays the music we have
    /// </summary>
    private void PlayMusic()
    {
        AudioManagerOptions audioManagerOptions = AudioManagerOptions.Default;
        audioManagerOptions.AudioManagerTrack = AudioManager.AudioManagerTracks.Music;
        audioManagerOptions.Loop = true;
        audioManagerOptions.Volume = volume;

        //AudioManager.instance.PlaySound(musicToChangeTo, audioManagerOptions);
        AudioManagerPlaySoundEvent.Trigger(music, audioManagerOptions);
    }

    /// <summary>
    /// Calls to he audio manager, and fade out the music in question
    /// </summary>
    private void StopMusic()
    {
        AudioSource audioSource = AudioManager.instance.FindByClip(music);
        if (audioSource)
        {
            AudioManager.instance.FadeSound(audioSource, fadeDuration, audioSource.volume, 0f, new TweenType(Tweens.TweenCurve.EaseOutExponential));
        }
    }
}
