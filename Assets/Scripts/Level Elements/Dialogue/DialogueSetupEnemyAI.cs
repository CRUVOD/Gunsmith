using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables AI cores on setup, then releases them/activates them after if needed
/// </summary>
public class DialogueSetupEnemyAI : DialogueSetupAction
{
    public bool activateAIOnDialogueEnd;
    //List of enemies affected by this script
    public AICore[] enemiesAffected;

    public override void Setup()
    {
        for (int i = 0; i < enemiesAffected.Length; i++)
        {
            enemiesAffected[i].gameObject.SetActive(false);
        }
    }

    public override void Release()
    {
        if (activateAIOnDialogueEnd)
        {
            for (int i = 0; i < enemiesAffected.Length; i++)
            {
                enemiesAffected[i].gameObject.SetActive(true);
            }
        }
    }
}
