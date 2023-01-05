using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This feedback will send a cameraShake event when played
/// </summary>
public class FeedbackCameraShake : Feedback
{
    [Header("Camera Shake")]
    /// whether or not this shake should repeat forever, until stopped
    [Tooltip("whether or not this shake should repeat forever, until stopped")]
    public bool RepeatUntilStopped = false;
    /// the channel to broadcast this shake on
    [Tooltip("the channel to broadcast this shake on")]
    public int Channel = 0;
    /// the properties of the shake (duration, intensity, frequenc)
    [Tooltip("the properties of the shake (duration, intensity, frequenc)")]
    public CameraShakeProperties CameraShakeProperties = new CameraShakeProperties(0.1f, 0.2f, 40f);

    /// the duration of this feedback is the duration of the shake
    public override float FeedbackDuration { get { return CameraShakeProperties.Duration; } set { CameraShakeProperties.Duration = value; } }

    /// <summary>
    /// On Play, sends a shake camera event
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
        CameraShakeEvent.Trigger(FeedbackDuration, CameraShakeProperties.Amplitude * intensityMultiplier, CameraShakeProperties.Frequency,
            CameraShakeProperties.AmplitudeX * intensityMultiplier, CameraShakeProperties.AmplitudeY * intensityMultiplier, CameraShakeProperties.AmplitudeZ * intensityMultiplier,
            RepeatUntilStopped, Channel, Timing.TimescaleMode == TimescaleModes.Unscaled);
    }

    protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        if (!Active)
        {
            return;
        }
        base.CustomStopFeedback(position, feedbacksIntensity);
        CameraShakeStopEvent.Trigger(Channel);
    }
}
