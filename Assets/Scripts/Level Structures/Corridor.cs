using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the event inside this corridor, corridors could be the place to place treasure chests
/// </summary>
public class Corridor : MonoBehaviour
{
    [Header("Level And Room")]
    public Collider2D RoomSize;
    public LevelConnection[] connections;
}
