using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponAction : AIAction
{
    public Weapon weapon;

    public override void PerformAction()
    {
        weapon.Use();
    }

    public override void OnExitState()
    {
        base.OnExitState();
        weapon.StopWeapon();
    }
}
