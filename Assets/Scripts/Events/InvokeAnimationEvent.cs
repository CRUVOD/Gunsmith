using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach this script to an animator, this script will then trigger the event specified in an animation event
/// </summary>
public class InvokeAnimationEvent : MonoBehaviour
{
    public UnityEvent myUnityEvent;

    public void AnimationEvent()
    {
        myUnityEvent.Invoke();
    }
}
