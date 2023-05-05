using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    [Tooltip("Indicates if this dialogue leads to another, prevents UI weirdness")]
    public bool hasSubsequentDialogue;
    private DialogueAction[] dialogueSetupActions;

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
        dialogueSetupActions = GetComponents<DialogueAction>();
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
        if (!hasSubsequentDialogue)
        {
            //No further dialogue following this one, we can safely tell dialogue system to resume noraml gameplay
            DialogueSystem.instance.EndDialogue();
        }

        for (int i = 0; i < dialogueSetupActions.Length; i++)
        {
            dialogueSetupActions[i].Release();
        }
    }
}
