using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for scripts that set up gameobjects for timelines that are triggered by player enter
/// </summary>
public class TimelineSetupAction : MonoBehaviour
{
    //How long the action lasts for
    public float duration;
    //The offset before the action takes place after trigger
    public float offset;

    /// <summary>
    /// Triggers the setup action after the offset
    /// </summary>
    public void TriggerAction(TimelinePlayerEnterTrigger timelineTrigger)
    {
        StopAllCoroutines();
        StartCoroutine(WaitForOffset(timelineTrigger));
    }

    /// <summary>
    /// Waits for the offset timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForOffset(TimelinePlayerEnterTrigger timelineTrigger)
    {
        yield return new WaitForSeconds(offset);
        Action(timelineTrigger);
    }

    protected virtual void Action(TimelinePlayerEnterTrigger timelineTrigger)
    {

    }
}
