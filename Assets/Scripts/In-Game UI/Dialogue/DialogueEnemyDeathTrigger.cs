using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assign a specific enemy's death to trigger dialogue
/// </summary>
public class DialogueEnemyDeathTrigger : MonoBehaviour
{
    public Enemy enemy;
    public DialogueTrigger dialogueTrigger;
    //The delay before the dialogue is triggered after enemy death
    public float delayBeforeDialogueTrigger;

    private void Start()
    {
        enemy.OnDeath += EnemyDeath;
    }

    private void EnemyDeath(Enemy enemy)
    {
        StopAllCoroutines();
        StartCoroutine(TriggerDialogue());
    }

    private IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(delayBeforeDialogueTrigger);
        dialogueTrigger.TriggerDialogue();
    }
}
