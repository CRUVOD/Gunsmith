using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMoveTowardsTarget : AIAction
{
    public NavMeshAgent ag;

    public Rigidbody2D rb;

    /// <summary>
    /// Make the target the player, this can be changed later to like decoy or something
    /// This script will take control over the defualt enemy movement on entering
    /// </summary>
    public override void Initialisation()
    {
        base.Initialisation();
        core.Target = LevelManager.instance.player.transform;
        ag.updateRotation = false;
        ag.updateUpAxis = false;
    }

    private void Update()
    {
        if (ActionInProgress && core.CoreActive && ag.enabled)
        {
            if (ag.remainingDistance > ag.stoppingDistance)
            {
                // Move rigibody by agent velocity and update agent position
                core.Owner.Move(ag.velocity);
                ag.nextPosition = rb.position;
            }
        }
        //CheckAgentStatus();
    }

    /// <summary>
    /// Pauses or resumes the navmesh agent based on if action and core are both active and true
    /// </summary>
    private void CheckAgentStatus()
    {
        if (!ActionInProgress || !core.CoreActive)
        {
            ag.enabled = false;
            core.Owner.externalMovementControl = false;
        }
        else if (ActionInProgress && core.CoreActive)
        {
            ag.enabled = true;
            core.Owner.externalMovementControl = true;
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        //Take control over movement
        core.Owner.externalMovementControl = true;
        ag.enabled = true;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        //Rescind control over movement
        core.Owner.externalMovementControl = false;
        ag.enabled = false;
    }

    public override void PerformAction()
    {
        UpdateTargetLocation();
    }

    #region HelperFunctions

    private void UpdateTargetLocation()
    {
        if (ag.enabled)
        {
            ag.SetDestination(core.Target.position);
        }
    }

    #endregion
}
