using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMoveTowardsPlayer : AIAction
{
    public NavMeshAgent ag;

    public Rigidbody2D rb;

    //bool that controls if the navmesh agent is disabled on exit of state
    public bool disableAgOnExit = true;

    /// <summary>
    /// Make the target the player, this can be changed later to like decoy or something
    /// This script will take control over the defualt enemy movement on entering
    /// </summary>
    public override void Initialisation()
    {
        base.Initialisation();
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
        ag.enabled = true;
    }

    public override void OnExitState()
    {
        base.OnExitState();
        if (disableAgOnExit)
        {
            ag.enabled = false;
        }
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
            ag.SetDestination(LevelManager.instance.player.transform.position);
        }
    }

    #endregion
}
