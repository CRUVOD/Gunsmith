using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathMusicChange : MonoBehaviour
{
    public AudioClip music;

    public float fadeDuration;

    public Enemy enemy;

    private void Start()
    {
        enemy.OnDeath += StopMusic;
    }

    /// <summary>
    /// Calls to he audio manager, and fade out the music in question
    /// </summary>
    private void StopMusic(Enemy enemy)
    {
        AudioSource audioSource = AudioManager.instance.FindByClip(music);
        if (audioSource)
        {
            AudioManager.instance.FadeSound(audioSource, fadeDuration, audioSource.volume, 0f, new TweenType(Tweens.TweenCurve.EaseOutExponential));
        }
    }
}
