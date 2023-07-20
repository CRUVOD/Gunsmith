using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIOnDamageDecision : AIDecision
{
    public Enemy enemy;

    bool hasTakenDamage;

    public override void Initialization()
    {
        base.Initialization();
        enemy.OnDamage += OnDamageTaken;
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        hasTakenDamage = false;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        //Reset return of Decide() on exit
        hasTakenDamage = false;
    }

    public override bool Decide()
    {
        return hasTakenDamage;
    }

    public void OnDamageTaken(float damage, Vector3 direction)
    {
        //If taken damage, we set the return of Decide() to true
        hasTakenDamage = true;
    }
}
