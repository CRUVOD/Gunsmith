using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Sprite potrait;

    public Dialogue dialogue;

    private DialogueSetupAction[] dialogueSetupActions;

    private void Awake()
    {
        GetDialogueSetupActions();
    }

    public void TriggerDialogue()
    {
        DialogueSystem.instance.StartDialogue(this);
    }

    private void GetDialogueSetupActions()
    {
        dialogueSetupActions = GetComponents<DialogueSetupAction>();
    }

    /// <summary>
    /// Executes all actions in dialogue setup
    /// </summary>
    public void Setup()
    {
        for (int i = 0; i < dialogueSetupActions.Length; i++)
        {
            dialogueSetupActions[i].Setup();
        }
    }

    /// <summary>
    /// Executes all release actions in dialogue setup
    /// </summary>
    public void Release()
    {
        for (int i = 0; i < dialogueSetupActions.Length; i++)
        {
            dialogueSetupActions[i].Release();
        }
    }
}
