using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackPlayer : MonoBehaviour
{
    /// the possible directions Feedbacks can be played
    public enum Directions { TopToBottom, BottomToTop }
    [Tooltip("the selected direction these feedbacks should play in")]
    public Directions Direction = Directions.TopToBottom;

    /// the intensity at which to play this feedback. That value will be used by most feedbacks to tune their amplitude. 1 is normal, 0.5 is half power, 0 is no effect.
    /// Note that what this value controls depends from feedback to feedback, don't hesitate to check the code to see what it does exactly.  
    [Tooltip("the intensity at which to play this feedback. That value will be used by most feedbacks to tune their amplitude. 1 is normal, 0.5 is half power, 0 is no effect." +
             "Note that what this value controls depends from feedback to feedback, don't hesitate to check the code to see what it does exactly.")]
    public float FeedbacksIntensity = 1f;

    /// whether or not this Feedbacks is playing right now - meaning it hasn't been stopped yet.
    /// if you don't stop your MMFeedbacks it'll remain true of course
    public bool IsPlaying { get; protected set; }
    /// if this MMFeedbacks is playing the time since it started playing
    public float ElapsedTime => IsPlaying ? GetTime() - _lastStartAt : 0f;
    /// the amount of times this MMFeedbacks has been played
    public int TimesPlayed { get; protected set; }
    /// a list of Feedback to trigger
    public List<Feedback> Feedbacks = new List<Feedback>();

    [Tooltip("the timescale at which the player itself will operate. This notably impacts sequencing and pauses duration evaluation.")]
    public TimescaleModes PlayerTimescaleMode = TimescaleModes.Unscaled;
    public virtual float GetTime() { return (PlayerTimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
    public virtual float GetDeltaTime() { return (PlayerTimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }
    /// The total duration (in seconds) of all the active feedbacks in this MMFeedbacks
    public virtual float TotalDuration
    {
        get
        {
            float total = 0f;
            foreach (Feedback feedback in Feedbacks)
            {
                if ((feedback != null) && (feedback.Active))
                {
                    if (total < feedback.TotalDuration)
                    {
                        total = feedback.TotalDuration;
                    }
                }
            }
            return total;
        }
    }

    protected float _startTime = 0f;
    protected float _holdingMax = 0f;
    protected float _lastStartAt = -float.MaxValue;


    private void Awake()
    {
        Initialisation(this.gameObject);
    }

    /// <summary>
    /// Initializes the MMFeedbacks, setting this MMFeedbacks as the owner
    /// </summary>
    public virtual void Initialisation()
    {
        Initialisation(this.gameObject);
    }

    /// <summary>
    /// A public method to initialize the feedback, specifying an owner that will be used as the reference for position and hierarchy by feedbacks
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="feedbacksOwner"></param>
    public virtual void Initialisation(GameObject owner)
    {
        IsPlaying = false;
        TimesPlayed = 0;
        _lastStartAt = -float.MaxValue;

        for (int i = 0; i < Feedbacks.Count; i++)
        {
            if (Feedbacks[i] != null)
            {
                Feedbacks[i].Initialisation(owner);
            }
        }
    }


    public virtual void PlayFeedbacks()
    {
        PlayFeedbacks(this.transform.position, FeedbacksIntensity);
    }

    public virtual void PlayFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
    {
        PlayAllFeedbacks(position, feedbacksIntensity, forceRevert);
    }

    public virtual void StopFeedbacks()
    {
        StopFeedbacks(this.transform.position, FeedbacksIntensity);
    }

    public virtual void StopFeedbacks(Vector3 position, float feedbacksIntensity = 1.0f, bool forceRevert = false)
    {
        StopAllFeedbacks(position, feedbacksIntensity, forceRevert);
    }


    /// <summary>
    /// This will return true if the conditions defined in the specified feedback's Timing section allow it to play in the current play direction of this MMFeedbacks
    /// </summary>
    /// <param name="feedback"></param>
    /// <returns></returns>
    protected bool FeedbackCanPlay(Feedback feedback)
    {
        if (feedback.Timing.MMFeedbacksDirectionCondition == FeedbackTiming.FeedbacksDirectionConditions.Always)
        {
            return true;
        }
        else if (((Direction == Directions.TopToBottom) && (feedback.Timing.MMFeedbacksDirectionCondition == FeedbackTiming.FeedbacksDirectionConditions.OnlyWhenForwards))
                 || ((Direction == Directions.BottomToTop) && (feedback.Timing.MMFeedbacksDirectionCondition == FeedbackTiming.FeedbacksDirectionConditions.OnlyWhenBackwards)))
        {
            return true;
        }
        return false;
    }

    // Plays all feedbacks
    protected virtual void PlayAllFeedbacks(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
    {
        // we just play all feedbacks at once
        for (int i = 0; i < Feedbacks.Count; i++)
        {
            if (FeedbackCanPlay(Feedbacks[i]))
            {
                Feedbacks[i].Play(position, feedbacksIntensity);
            }
        }
    }

    protected virtual void StopAllFeedbacks(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
    {
        // Try stopping feedbacks to play
        for (int i = 0; i < Feedbacks.Count; i++)
        {
            Feedbacks[i].Stop(position, feedbacksIntensity);
        }
    }

    /// <summary>
    /// Calls each feedback's Reset method if they've defined one. An example of that can be resetting the initial color of a flickering renderer.
    /// </summary>
    public virtual void ResetFeedbacks()
    {
        for (int i = 0; i < Feedbacks.Count; i++)
        {
            if ((Feedbacks[i] != null) && (Feedbacks[i].Active))
            {
                Feedbacks[i].ResetFeedback();
            }
        }
        IsPlaying = false;
    }
}
