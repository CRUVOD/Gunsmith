using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICastWeaponAction : AIAction
{
    public CastWeapon castWeapon;

    private void Update()
    {
        if (ActionInProgress)
        {
            castWeapon.Use();
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        castWeapon.StopWeapon();
    }


    public override void PerformAction()
    {
        //We instead use update for this action
        return;
    }
}
