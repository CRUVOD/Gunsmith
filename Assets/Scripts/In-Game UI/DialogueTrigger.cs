using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Sprite potrait;

    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        DialogueSystem.instance.StartDialogue(dialogue, potrait);
    }
}
