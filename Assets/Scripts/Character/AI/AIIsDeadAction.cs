using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIIsDeadAction : AIAction
{
    public NavMeshAgent ag;
    //Do nothing, the enemy is dead

    public override void OnEnterState()
    {
        //Disable AI behaviours
        core.CoreActive = false;
        ag.enabled = false;
    }

    public override void PerformAction()
    {
        return;
    }

}
