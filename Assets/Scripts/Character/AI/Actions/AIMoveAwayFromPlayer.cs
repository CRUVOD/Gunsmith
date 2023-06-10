using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMoveAwayFromPlayer : AIAction
{
    public NavMeshAgent ag;

    public Rigidbody2D rb;

    [Tooltip("Mininum distance from a wall the AI should consider")]
    public float minDistanceToWall;

    /// <summary>
    /// Make the target the player, this can be changed later to like decoy or something
    /// This script will take control over the defualt enemy movement on entering
    /// </summary>
    public override void Initialisation()
    {
        base.Initialisation();
        //core.TargetPosition = GetNewTargetLocation();
        ag.updateRotation = false;
        ag.updateUpAxis = false;
    }

    /// <summary>
    /// Pauses or resumes the navmesh agent based on if action and core are both active and true
    /// </summary>
    private void CheckAgentStatus()
    {
        if (!ActionInProgress || !core.CoreActive)
        {
            ag.enabled = false;
        }
        else if (ActionInProgress && core.CoreActive)
        {
            ag.enabled = true;
        }
    }

    public override void OnEnterState()
    {
        base.OnEnterState();
        core.TargetPosition = LevelManager.instance.player.transform.position;
        ag.enabled = true;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        ag.enabled = false;
    }

    public override void PerformAction()
    {
        //if target location is still within distance of the player, set a new location
        core.TargetPosition = GetNewTargetLocation();
        if (ag.enabled)
        {
            ag.SetDestination(core.TargetPosition);
        }

    }

    /// <summary>
    /// Updates the core's target location to be away from the player
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNewTargetLocation()
    {
        //We will need to rotate the direction away from the player if straight to the opposite of the player is a wall
        float vRotation = 0;

        while (vRotation < 360)
        {
            //Calculate the vector pointing from Player to the Enemy
            Vector3 dirToPlayer = (transform.position - LevelManager.instance.player.transform.position).normalized;

            Vector3 rayDir = Quaternion.Euler(0, 0, vRotation) * dirToPlayer;

            bool isHit = Physics2D.Raycast(transform.position, rayDir, minDistanceToWall, LayerManager.ObstaclesLayerMask);
            
            if (!isHit)
            {
                return transform.position + rayDir * minDistanceToWall;
            }

            vRotation += 20;
        }

        //Stay still if theres no places to go
        return transform.position;
    }

}
