using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple script which prompts the text input after dialogue finish, and saves the player name entered
/// </summary>
public class DialogueTriggerSavePlayerName : DialogueAction
{
    [Tooltip("If at the end of this save, trigger another dialogue")]
    public bool triggerDialogue;
    public DialogueTrigger subsequentDialogue;

    public override void Release()
    {
        TextInputSystem.Prompt("Name", SavePlayerName);
    }

    public void SavePlayerName(string name)
    {
        PlayerData playerData = new PlayerData(name);
        SaveSystem.SavePlayer(playerData);

        if (triggerDialogue)
        {
            DialogueSystem.instance.StartDialogue(subsequentDialogue);
        }
    }
}
