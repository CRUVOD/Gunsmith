using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSetupEnemyFacing : DialogueAction
{
    //List of enemies affected by this script
    public Enemy[] enemies;
    //List of directions that the enemies will be facing
    public bool[] enemyFacing;

    public override void Setup()
    {
        if (enemies.Length != enemyFacing.Length)
        {
            Debug.Log("Mismatch in length, dialogue setup failed");
            return;
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].ForceFacing(true, enemyFacing[i]);
        }
    }

    public override void Release()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].ForceFacing(false);
        }
    }
}
