using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This feedback will play the associated particles system on play, and stop it on stop
/// </summary>
[AddComponentMenu("")]
[FeedbackHelp("This feedback will simply play the specified ParticleSystem (from your scene) when played.")]
[FeedbackPath("Particles/Particles Play")]
public class FeedbackParticles : Feedback
{
    public enum Modes { Play, Stop, Pause }

    [Header("Bound Particles")]
    /// whether to Play, Stop or Pause the target particle system when that feedback is played
    [Tooltip("whether to Play, Stop or Pause the target particle system when that feedback is played")]
    public Modes Mode = Modes.Play;
    /// the particle system to play with this feedback
    [Tooltip("the particle system to play with this feedback")]
    public ParticleSystem BoundParticleSystem;
    /// a list of (optional) particle systems 
    [Tooltip("a list of (optional) particle systems")]
    public List<ParticleSystem> RandomParticleSystems;
    /// if this is true, the particles will be moved to the position passed in parameters
    [Tooltip("if this is true, the particles will be moved to the position passed in parameters")]
    public bool MoveToPosition = false;
    /// if this is true, the particle system's object will be set active on play
    [Tooltip("if this is true, the particle system's object will be set active on play")]
    public bool ActivateOnPlay = false;
    [Tooltip("typically this represents how long should the particles play")]
    public float particleDuration;

    /// the duration of this feedback is a custom value set by the user, usually meaning how long should the particles play
    public override float FeedbackDuration { get { return GetDuration(); } }

    private float GetDuration()
    {
        return particleDuration;
    }

    /// <summary>
    /// On init we stop our particle system
    /// </summary>
    /// <param name="owner"></param>
    protected override void CustomInitialisation(GameObject owner)
    {
        base.CustomInitialisation(owner);
        StopParticles();
    }

    /// <summary>
    /// On play we play our particle system
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (!Active)
        {
            return;
        }
        PlayParticles(position);
    }

    /// <summary>
    /// On Stop, stops the particle system
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (!Active)
        {
            return;
        }
        StopParticles();
    }

    /// <summary>
    /// On Reset, stops the particle system 
    /// </summary>
    protected override void CustomReset()
    {
        base.CustomReset();

        if (InCooldown)
        {
            return;
        }

        StopParticles();
    }

    /// <summary>
    /// Plays a particle system
    /// </summary>
    /// <param name="position"></param>
    protected virtual void PlayParticles(Vector3 position)
    {
        if (MoveToPosition)
        {
            BoundParticleSystem.transform.position = position;
            foreach (ParticleSystem system in RandomParticleSystems)
            {
                system.transform.position = position;
            }
        }

        if (ActivateOnPlay)
        {
            BoundParticleSystem.gameObject.SetActive(true);
            foreach (ParticleSystem system in RandomParticleSystems)
            {
                system.gameObject.SetActive(true);
            }
        }

        if (RandomParticleSystems.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, RandomParticleSystems.Count);
            switch (Mode)
            {
                case Modes.Play:
                    RandomParticleSystems[random].Play();
                    break;
                case Modes.Stop:
                    RandomParticleSystems[random].Stop();
                    break;
                case Modes.Pause:
                    RandomParticleSystems[random].Pause();
                    break;
            }
            return;
        }
        else if (BoundParticleSystem != null)
        {
            switch (Mode)
            {
                case Modes.Play:
                    BoundParticleSystem?.Play();
                    break;
                case Modes.Stop:
                    BoundParticleSystem?.Stop();
                    break;
                case Modes.Pause:
                    BoundParticleSystem?.Pause();
                    break;
            }
        }
    }

    /// <summary>
    /// Stops all particle systems
    /// </summary>
    protected virtual void StopParticles()
    {
        foreach (ParticleSystem system in RandomParticleSystems)
        {
            system?.Stop();
        }
        if (BoundParticleSystem != null)
        {
            BoundParticleSystem.Stop();
        }
    }
}
