using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// This feedback will turn a target light on or off for a set duration
/// </summary>
public class FeedbackLight : Feedback
{
    public enum Modes {On, Off}

    [Header("Light")]
    public Modes Mode = Modes.On;
    public Light2D lightToControl;

    [Tooltip("this represents how long should the light be active/inactive for")]
    public float lightDuration;

    Coroutine lightCoroutine;

    /// <summary>
    /// Turn the light on or off at the start based on selected parameteres
    /// </summary>
    /// <param name="owner"></param>
    protected override void CustomInitialisation(GameObject owner)
    {
        base.CustomInitialisation(owner);
        if (Mode == Modes.Off)
        {
            lightToControl.enabled = true;
        }
        else
        {
            lightToControl.enabled = false;
        }
    }

    /// <summary>
    /// Turn on or off the light for a set duration
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        lightCoroutine = StartCoroutine(ManipulateLight());
    }

    public virtual IEnumerator ManipulateLight()
    {
        IsPlaying = true;

        if (Mode == Modes.Off)
        {
            lightToControl.enabled = false;
            yield return FeedbacksCoroutine.WaitFor(lightDuration);
            lightToControl.enabled = true;
        }
        else
        {
            lightToControl.enabled = true;
            yield return FeedbacksCoroutine.WaitFor(lightDuration);
            lightToControl.enabled = false;
        }

        IsPlaying = false;
    }

    /// <summary>
    /// Stops this feedback
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
    {
        if (!Active)
        {
            return;
        }
        base.CustomStopFeedback(position, feedbacksIntensity);

        IsPlaying = false;
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }
    }
}
