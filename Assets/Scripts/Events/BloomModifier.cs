using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Add this class to a Camera with a bloom post processing and it'll be able to "shake" its values by getting events
/// </summary>
[RequireComponent(typeof(Volume))]
public class BloomModifier : ValueModifier
{
    /// whether or not to add to the initial value
    public bool RelativeValues = true;

    [Header("Bloom Intensity")]
    /// the curve used to animate the intensity value on
    [Tooltip("the curve used to animate the intensity value on")]
    public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
    /// the value to remap the curve's 0 to
    [Tooltip("the value to remap the curve's 0 to")]
    public float RemapIntensityZero = 0f;
    /// the value to remap the curve's 1 to
    [Tooltip("the value to remap the curve's 1 to")]
    public float RemapIntensityOne = 10f;

    [Header("Bloom Threshold")]
    /// the curve used to animate the threshold value on
    [Tooltip("the curve used to animate the threshold value on")]
    public AnimationCurve ShakeThreshold = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
    /// the value to remap the curve's 0 to
    [Tooltip("the value to remap the curve's 0 to")]
    public float RemapThresholdZero = 0f;
    /// the value to remap the curve's 1 to
    [Tooltip("the value to remap the curve's 1 to")]
    public float RemapThresholdOne = 0f;

    protected Volume postProcessVolume;
    protected Bloom bloom;
    protected float initialIntensity;
    protected float initialThreshold;
    protected float originalShakeDuration;
    protected bool originalRelativeIntensity;
    protected AnimationCurve originalShakeIntensity;
    protected float originalRemapIntensityZero;
    protected float originalRemapIntensityOne;
    protected AnimationCurve originalShakeThreshold;
    protected float originalRemapThresholdZero;
    protected float originalRemapThresholdOne;


    /// <summary>
    /// On init we initialize our values
    /// </summary>
    protected override void Initialization()
    {
        
        base.Initialization();
        postProcessVolume = this.gameObject.GetComponent<Volume>();
        postProcessVolume.profile.TryGet<Bloom>(out bloom);
    }

    /// <summary>
    /// Shakes values over time
    /// </summary>
    protected override void Shake()
    {
        float newIntensity = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeValues, initialIntensity);
        bloom.intensity.Override(newIntensity);
        float newThreshold = ShakeFloat(ShakeThreshold, RemapThresholdZero, RemapThresholdOne, RelativeValues, initialThreshold);
        bloom.threshold.Override(newThreshold);
    }

    /// <summary>
    /// Collects initial values on the target
    /// </summary>
    protected override void GrabInitialValues()
    {
        initialIntensity = (float) bloom.intensity;
        initialThreshold = (float) bloom.threshold;
    }

    /// <summary>
    /// When we get the appropriate event, we trigger a shake
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="duration"></param>
    /// <param name="amplitude"></param>
    /// <param name="relativeIntensity"></param>
    /// <param name="feedbacksIntensity"></param>
    /// <param name="channel"></param>
    public virtual void OnBloomShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax,
        AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false,
        float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
    {
        if (!CheckEventAllowed(channel) || (!Interruptible && Shaking))
        {
            return;
        }

        if (stop)
        {
            Stop();
            return;
        }

        _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
        _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

        if (resetShakerValuesAfterShake)
        {
            originalShakeDuration = ShakeDuration;
            originalShakeIntensity = ShakeIntensity;
            originalRemapIntensityZero = RemapIntensityZero;
            originalRemapIntensityOne = RemapIntensityOne;
            originalRelativeIntensity = RelativeValues;
            originalShakeThreshold = ShakeThreshold;
            originalRemapThresholdZero = RemapThresholdZero;
            originalRemapThresholdOne = RemapThresholdOne;
        }

        TimescaleMode = timescaleMode;
        ShakeDuration = duration;
        ShakeIntensity = intensity;
        RemapIntensityZero = remapMin * feedbacksIntensity;
        RemapIntensityOne = remapMax * feedbacksIntensity;
        RelativeValues = relativeIntensity;
        ShakeThreshold = threshold;
        RemapThresholdZero = remapThresholdMin;
        RemapThresholdOne = remapThresholdMax;
        ForwardDirection = forwardDirection;

        Play();
    }

    /// <summary>
    /// Resets the target's values
    /// </summary>
    protected override void ResetTargetValues()
    {
        base.ResetTargetValues();
        bloom.intensity.Override(initialIntensity);
        bloom.threshold.Override(initialThreshold);
    }

    /// <summary>
    /// Resets the shaker's values
    /// </summary>
    protected override void ResetShakerValues()
    {
        base.ResetShakerValues();
        ShakeDuration = originalShakeDuration;
        ShakeIntensity = originalShakeIntensity;
        RemapIntensityZero = originalRemapIntensityZero;
        RemapIntensityOne = originalRemapIntensityOne;
        RelativeValues = originalRelativeIntensity;
        ShakeThreshold = originalShakeThreshold;
        RemapThresholdZero = originalRemapThresholdZero;
        RemapThresholdOne = originalRemapThresholdOne;
    }

    /// <summary>
    /// Starts listening for events
    /// </summary>
    public override void StartListening()
    {
        base.StartListening();
        BloomModifyEvent.Register(OnBloomShakeEvent);
    }

    /// <summary>
    /// Stops listening for events
    /// </summary>
    public override void StopListening()
    {
        base.StopListening();
        BloomModifyEvent.Unregister(OnBloomShakeEvent);
    }
}

/// <summary>
/// An event used to trigger vignette shakes
/// </summary>
public struct BloomModifyEvent
{
    public delegate void Delegate(AnimationCurve intensity, float duration, float remapMin, float remapMax,
        AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false,
        float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);
    static private event Delegate OnEvent;

    static public void Register(Delegate callback)
    {
        OnEvent += callback;
    }

    static public void Unregister(Delegate callback)
    {
        OnEvent -= callback;
    }

    static public void Trigger(AnimationCurve intensity, float duration, float remapMin, float remapMax,
        AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false,
        float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
    {
        OnEvent?.Invoke(intensity, duration, remapMin, remapMax, threshold, remapThresholdMin, remapThresholdMax, relativeIntensity,
            feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
    }
    }
