using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Timeline set up action that moves the player into position in 'duration' amount of time
/// </summary>
public class TimelineSetupPlayerMove : TimelineSetupAction
{
    //Moves the player first to the start position in this set amount of time then triggers the timline
    public Vector3 timelineStartPos;
    private Player player;

    /// <summary>
    /// Moves the player to the correct position to play the timline
    /// </summary>
    protected override void Action(TimelinePlayerEnterTrigger timelineTrigger)
    {
        this.player = timelineTrigger.GetPlayer();
        if (player == null)
        {
            return;
        }
        Vector2 direction = (timelineStartPos - player.transform.position).normalized;
        float speed = (timelineStartPos - player.transform.position).magnitude / duration;
        player.ControlPlayerMovement(direction, speed, duration);
    }
}
