using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIIsDeadDecision : AIDecision
{
    public Enemy enemy;

    public override bool Decide()
    {
        return CheckDeath();
    }

    private bool CheckDeath()
    {
        return (enemy.CurrentHealth <= 0);
    }
}
