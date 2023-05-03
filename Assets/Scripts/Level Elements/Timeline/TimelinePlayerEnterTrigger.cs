using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelinePlayerEnterTrigger : MonoBehaviour
{
    //This ensures that the timeline only triggers once, can change this to multiple times later
    private bool hasBeenTriggered;
    public TimelineReference timelineReference;
    [HideInInspector]
    //The amount of time before triggering the timeline
    public float delayTime;
    //List of actions that needs to be executed before starting the timeline
    private TimelineSetupAction[] timelineSetupActions;
    private Player player;

    private void Awake()
    {
        hasBeenTriggered = false;
        GetSetupActions();
        SetDelayTime();
    }

    private void GetSetupActions()
    {
        timelineSetupActions = GetComponents<TimelineSetupAction>();
    }

    /// <summary>
    /// Sets the delay time before triggering timeline to be the longest lasting action in the list
    /// </summary>
    private void SetDelayTime()
    {
        delayTime = 0;
        for (int i = 0; i < timelineSetupActions.Length; i++)
        {
            if (timelineSetupActions[i].duration + timelineSetupActions[i].offset > delayTime)
            {
                delayTime = timelineSetupActions[i].duration + timelineSetupActions[i].offset;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !hasBeenTriggered)
        {
            player = collision.gameObject.GetComponent<Player>();
            hasBeenTriggered = true;
            SetupTimeline();
            StartCoroutine(WaitTillTriggerTimeline());
        }
    }

    /// <summary>
    /// Triggers all setup actions
    /// </summary>
    private void SetupTimeline()
    {
        for (int i = 0; i < timelineSetupActions.Length; i++)
        {
            timelineSetupActions[i].TriggerAction(this);
        }
    }

    public Player GetPlayer()
    {
        return player;
    }

    IEnumerator WaitTillTriggerTimeline()
    {
        yield return new WaitForSeconds(delayTime + 0.01f);
        TriggerTimeline();
    }

    public void TriggerTimeline()
    {
        TimelineManager.instance.PlayTimeline(timelineReference.ID);
    }
}
