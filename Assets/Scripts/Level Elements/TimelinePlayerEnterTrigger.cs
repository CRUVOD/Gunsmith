using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelinePlayerEnterTrigger : MonoBehaviour
{
    //This ensures that the timeline only triggers once, can change this to multiple times later
    private bool hasBeenTriggered;
    public TimelineReference timelineReference;
    //Moves the player first to the start position in this set amount of time then triggers the timline
    public float movePlayerDuration;
    public Vector3 timelineStartPos;
    private Player player;

    private void Awake()
    {
        hasBeenTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !hasBeenTriggered)
        {
            player = collision.gameObject.GetComponent<Player>();
            hasBeenTriggered = true;
            MovePlayer();
        }
    }

    /// <summary>
    /// Moves the player to the correct position to play the timline
    /// </summary>
    void MovePlayer()
    {
        StopAllCoroutines();
        Vector2 direction = (timelineStartPos - player.transform.position).normalized;
        float speed = (timelineStartPos - player.transform.position).magnitude / movePlayerDuration;
        player.ControlPlayerMovement(direction, speed, movePlayerDuration);
        StartCoroutine(WaitTillTriggerTimeline());
    }

    IEnumerator WaitTillTriggerTimeline()
    {
        //Wait till playermovement is finished
        yield return new WaitForSeconds(movePlayerDuration + 0.001f);
        TriggerTimeline();
    }

    public void TriggerTimeline()
    {
        TimelineManager.instance.PlayTimeline(timelineReference.ID);
    }
}
