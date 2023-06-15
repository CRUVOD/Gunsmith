using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Decision will return true if AI has line of sight on player and is within max distance
/// </summary>
public class AIPlayerLineOfSightDecision : AIDecision
{
    [Tooltip("the max distance to be away from player")]
    public float maxDistance;

    /// <summary>
    /// On Decide we check our distance to the Target
    /// </summary>
    /// <returns></returns>
    public override bool Decide()
    {
        if (LevelManager.instance.player == null)
        {
            //If there is no player, safe return early
            return false;
        }

        return EvaluateDistance() && EvaluateLineOfSight();
    }

    private bool EvaluateLineOfSight()
    {
        LayerMask layerMask = LayerManager.ObstaclesLayerMask | LayerManager.PlayerLayerMask;
        Vector3 playerDir = (LevelManager.instance.player.transform.position - transform.position).normalized;
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, playerDir, 10000f, layerMask);
        return (raycastHit.collider.tag == "Player");
    }

    private bool EvaluateDistance()
    {
        float distance = Vector3.Distance(this.transform.position, LevelManager.instance.player.transform.position);

        return distance < maxDistance;
    }
}
