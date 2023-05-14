using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerMusicChange : DialogueAction
{
    /// <summary>
    /// When should the music change take place, when the dialogue starts, or when it ends
    /// </summary>
    public enum DialogueTriggerMusicChangeTime { Start, End}
    public DialogueTriggerMusicChangeTime musicChangeTime;

    public AudioClip musicToChangeTo;
    [Range(0f, 2f)]
    public float volume;

    public override void Setup()
    {
        if (musicChangeTime == DialogueTriggerMusicChangeTime.Start)
        {
            PlayMusic();
        }
    }

    public override void Release()
    {
        if (musicChangeTime == DialogueTriggerMusicChangeTime.End)
        {
            PlayMusic();
        }
    }

    private void PlayMusic()
    {
        AudioManagerOptions audioManagerOptions = AudioManagerOptions.Default;
        audioManagerOptions.AudioManagerTrack = AudioManager.AudioManagerTracks.Music;
        audioManagerOptions.Loop = true;
        audioManagerOptions.Volume = volume;

        //AudioManager.instance.PlaySound(musicToChangeTo, audioManagerOptions);
        AudioManagerPlaySoundEvent.Trigger(musicToChangeTo, audioManagerOptions);
    }
}
