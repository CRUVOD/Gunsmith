using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This AI decision will return true if the navmesh agent of the enemy has reached its destination/close enough it doesn't matter
/// </summary>
public class AIMovementCompleteDecision : AIDecision
{
    public NavMeshAgent ag;

    //How close should the AI be to the target decision before we say "its close enough"
    public float tolerance = 0.15f;
    //The time delay before we check if the navmesh agent has reached its destination, this is to prevent checking too quickly and staying on the spot
    public float delayBeforeCheck = 0.1f;
    float delayTimer;

    public override void Initialization()
    {
        base.Initialization();
        //Makes sure that the navmesh agent can actually reach within the tolerance
        if (ag.stoppingDistance > tolerance)
        {
            ag.stoppingDistance = Mathf.Clamp(tolerance - 0.2f, 0, ag.stoppingDistance);
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        delayTimer = delayBeforeCheck;
    }

    public override bool Decide()
    {
        delayTimer -= Time.deltaTime;
        if (delayTimer <= 0)
        {
            return (ag.remainingDistance <= tolerance);
        }
        else
        {
            return false;
        }
    }
}
