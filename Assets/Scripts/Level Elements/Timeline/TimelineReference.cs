using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[SerializeField]
[RequireComponent(typeof(PlayableDirector))]
public class TimelineReference : MonoBehaviour
{
    public string ID;
    public PlayableDirector timelineDirector;
    //Determines if a dialogue is triggered at the end of the timeline
    public bool triggerDialogue;
    public DialogueTrigger dialogue;
}
