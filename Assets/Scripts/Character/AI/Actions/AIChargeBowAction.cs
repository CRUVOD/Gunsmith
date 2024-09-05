using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChargeBowAction : AIAction
{
    public ChargeBow chargeBow;

    private void Update()
    {
        if (ActionInProgress)
        {
            chargeBow.Use();
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        chargeBow.StopWeapon();
    }

    public override void PerformAction()
    {
        //We instead use update for this action
        return;
    }
}
