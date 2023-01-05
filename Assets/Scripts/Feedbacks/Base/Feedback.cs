using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Feedback : MonoBehaviour
{
    /// whether or not this feedback is active
    [Tooltip("whether or not this feedback is active")]
    public bool Active = true;
    /// the name of this feedback to display in the inspector
    [Tooltip("the name of this feedback to display in the inspector")]
    public string Label = "Feedback";
    /// the chance of this feedback happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)
    [Tooltip("the chance of this feedback happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)")]
    [Range(0, 100)]
    public float Chance = 100f;
    [Tooltip("a number of timing-related values (delay, repeat, etc)")]
    public FeedbackTiming Timing;
    /// the Owner of the feedback, as defined when calling the Initialization method
    public GameObject Owner { get; set; }

    // the timestamp at which this feedback was last played
    public virtual float FeedbackStartedAt { get { return _lastPlayTimestamp; } }
    // the perceived duration of the feedback, to be used to display its progress bar, meant to be overridden with meaningful data by each feedback
    public virtual float FeedbackDuration { get { return 0f; } set { } }
    /// whether or not this feedback is playing right now
    public virtual bool FeedbackPlaying { get { return ((FeedbackStartedAt > 0f) && (Time.time - FeedbackStartedAt < FeedbackDuration)); } }
    /// a number of timing-related values (delay, repeat, etc)

    protected float _lastPlayTimestamp = -1f;
    protected int _playsLeft;
    protected bool _initialized = false;


    /// returns true if this feedback is in cooldown at this time (and thus can't play), false otherwise
    public virtual bool InCooldown { get { return (Timing.CooldownDuration > 0f) && (FeedbackTime - _lastPlayTimestamp < Timing.CooldownDuration); } }
    public virtual bool IsPlaying { get; set; }

    protected Coroutine _playCoroutine;
    protected Coroutine _infinitePlayCoroutine;
    protected Coroutine _repeatedPlayCoroutine;

    /// <summary>
    /// Initializes the feedback and its timing related variables
    /// </summary>
    /// <param name="owner"></param>
    public virtual void Initialisation(GameObject owner)
    {
        _initialized = true;
        Owner = owner;
        _playsLeft = Timing.NumberOfRepeats + 1;

        SetInitialDelay(Timing.InitialDelay);
        SetDelayBetweenRepeats(Timing.DelayBetweenRepeats);

        CustomInitialisation(owner);
    }

    /// the time (or unscaled time) based on the selected Timing settings
    public float FeedbackTime
    {
        get
        {
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                return Time.time;
            }
            else
            {
                return Time.unscaledTime;
            }
        }
    }

    /// the delta time (or unscaled delta time) based on the selected Timing settings
    public float FeedbackDeltaTime
    {
        get
        {
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                return Time.deltaTime;
            }
            else
            {
                return Time.unscaledDeltaTime;
            }
        }
    }


    /// <summary>
    /// The total duration of this feedback :
    /// total = initial delay + duration * (number of repeats + delay between repeats)  
    /// </summary>
    public float TotalDuration
    {
        get
        {
            if ((Timing != null) && (!Timing.ContributeToTotalDuration))
            {
                return 0f;
            }

            float totalTime = 0f;

            if (Timing == null)
            {
                return 0f;
            }

            if (Timing.InitialDelay != 0)
            {
                totalTime += Timing.InitialDelay;
            }

            totalTime += FeedbackDuration;

            if (Timing.NumberOfRepeats > 0)
            {
                float delayBetweenRepeats = Timing.DelayBetweenRepeats;

                totalTime += (Timing.NumberOfRepeats * FeedbackDuration) + (Timing.NumberOfRepeats * delayBetweenRepeats);
            }

            return totalTime;
        }
    }

    /// <summary>
    /// Plays the feedback
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    public virtual void Play(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (!Active)
        {
            return;
        }

        if (!_initialized)
        {
            Debug.LogWarning("The " + this + " feedback is being played without having been initialized. Call Initialization() first.");
        }

        // we check the cooldown
        if (InCooldown)
        {
            return;
        }

        if (Timing.InitialDelay > 0f)
        {
            _playCoroutine = StartCoroutine(PlayCoroutine(position, feedbacksIntensity));
        }
        else
        {
            _lastPlayTimestamp = FeedbackTime;
            RegularPlay(position, feedbacksIntensity);
        }
    }

    public virtual void Stop(Vector3 position, float feedbackIntensity)
    {
        CustomStopFeedback(position, feedbackIntensity);
    }

    /// <summary>
    /// An internal coroutine delaying the initial play of the feedback
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    /// <returns></returns>
    protected virtual IEnumerator PlayCoroutine(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (Timing.TimescaleMode == TimescaleModes.Scaled)
        {
            yield return FeedbacksCoroutine.WaitFor(Timing.InitialDelay);
        }
        else
        {
            yield return FeedbacksCoroutine.WaitForUnscaled(Timing.InitialDelay);
        }
        _lastPlayTimestamp = FeedbackTime;
        RegularPlay(position, feedbacksIntensity);
    }

    /// <summary>
    /// Triggers delaying coroutines if needed
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected virtual void RegularPlay(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (Chance == 0f)
        {
            return;
        }
        if (Chance != 100f)
        {
            // determine the odds
            float random = Random.Range(0f, 100f);
            if (random > Chance)
            {
                return;
            }
        }

        if (Timing.UseIntensityInterval)
        {
            if ((feedbacksIntensity < Timing.IntensityIntervalMin) || (feedbacksIntensity >= Timing.IntensityIntervalMax))
            {
                return;
            }
        }

        if (Timing.RepeatForever)
        {
            _infinitePlayCoroutine = StartCoroutine(InfinitePlay(position, feedbacksIntensity));
            return;
        }
        if (Timing.NumberOfRepeats > 0)
        {
            _repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(position, feedbacksIntensity));
            return;
        }

        CustomPlayFeedback(position, feedbacksIntensity);
        
    }
    /// <summary>
    /// Internal coroutine used for repeated play without end
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    /// <returns></returns>
    protected virtual IEnumerator InfinitePlay(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        while (true)
        {
            _lastPlayTimestamp = FeedbackTime;
            CustomPlayFeedback(position, feedbacksIntensity);
            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return FeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
            }
            else
            {
                yield return FeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
            }
        }
    }

    /// <summary>
    /// Internal coroutine used for repeated play
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    /// <returns></returns>
    protected virtual IEnumerator RepeatedPlay(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        while (_playsLeft > 0)
        {
            _lastPlayTimestamp = FeedbackTime;
            _playsLeft--;
            CustomPlayFeedback(position, feedbacksIntensity);

            if (Timing.TimescaleMode == TimescaleModes.Scaled)
            {
                yield return FeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
            }
            else
            {
                yield return FeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
            }

        }
        _playsLeft = Timing.NumberOfRepeats + 1;
    }

    /// <summary>
    /// Calls this feedback's custom reset 
    /// </summary>
    public virtual void ResetFeedback()
    {
        _playsLeft = Timing.NumberOfRepeats + 1;
        CustomReset();
    }

    /// <summary>
    /// This method describes what happens when the feedback gets played
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected abstract void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f);

    /// <summary>
    /// This method describes what happens when the feedback gets stopped
    /// </summary>
    /// <param name="position"></param>
    /// <param name="feedbacksIntensity"></param>
    protected virtual void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f) { }

    /// <summary>
    /// Use this method to specify a new initial delay at runtime
    /// </summary>
    /// <param name="delay"></param>
    public virtual void SetInitialDelay(float delay)
    {
        Timing.InitialDelay = delay;
    }

    /// <summary>
    /// Use this method to specify a new delay between repeats at runtime
    /// </summary>
    /// <param name="delay"></param>
    public virtual void SetDelayBetweenRepeats(float delay)
    {
        Timing.DelayBetweenRepeats = delay;
    }

    /// <summary>
    /// This method describes all custom initialization processes the feedback requires, in addition to the main Initialization method
    /// </summary>
    /// <param name="owner"></param>
    protected virtual void CustomInitialisation(GameObject owner) { }

    /// <summary>
    /// This method describes what happens when the feedback gets reset
    /// </summary>
    protected virtual void CustomReset() { }
}
