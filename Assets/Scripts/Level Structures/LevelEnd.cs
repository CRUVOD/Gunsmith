using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    [Header("Level And Room")]
    public BoxCollider2D RoomSize;
    public LevelConnection[] connections;
    public RoomPlayerDetector playerDetector;

    bool playerIsInRoom;

    private void Start()
    {
        //Listens to the playerdetector for events
        playerDetector.OnPlayerEnter += PlayerEnterHandler;
        playerDetector.OnPlayerExit += PlayerExitHandler;

        playerIsInRoom = false;
    }

    private void PlayerEnterHandler()
    {
        playerIsInRoom = true;
        GameEvent.Trigger(GameEvents.LevelEnd);
    }

    private void PlayerExitHandler()
    {
        playerIsInRoom = false;
    }
}
