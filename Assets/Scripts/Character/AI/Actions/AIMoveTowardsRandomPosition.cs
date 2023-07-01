using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This AI action will randomly pick a location on the NAvmesh within a certain radius to move to
/// </summary>
public class AIMoveTowardsRandomPosition : AIAction
{
    public NavMeshAgent ag;

    public Rigidbody2D rb;

    public float movementRadius;

    //bool that controls if the navmesh agent is disabled on exit of state
    public bool disableAgOnExit = true;

    //bool that makes sure we only set a random location once per EnterState()
    bool newLocationSet;

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
        newLocationSet = false;
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
        if (!newLocationSet)
        {
            Vector2 point = RandomNavmeshLocation(movementRadius);
            ag.SetDestination(point);
            newLocationSet = true;
        }
    }

    public Vector2 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector2 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }


}
