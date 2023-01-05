using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Randomly generates a level based on the provided avaialable modules/pieces. Rooms are always connected by corridors in between
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("Level Pieces")]
    public Room[] roomModules;
    public Corridor[] corridorModules;

    public Corridor StartingModule;

    [Header("LevelGen Settings")]
    //The nubmer of rooms for the player to complete in a straight path
    public int numberOfRoomsLinear;
    //The number of rooms in the level in total
    public int numberOfRoomsTotal;
    //keep track of how many rooms still needs to be generated
    int roomsToCreate;
    //Max attempts to try and connect a new module to another one before giving up and moving to the next one
    public int maxAttemptsAttachingRoom = 4;
    public int maxAttemptsAttachingCorridor = 4;
    

    private void Awake()
    {
        roomsToCreate = numberOfRoomsTotal;
    }

    private void Start()
    {
        GenerateLevel();
    }

    /// <summary>
    /// Depth first priority level generation? Like try to keep adding rooms one after the other until it can't anymore
    /// </summary>
    void GenerateLevel()
    {
        if (numberOfRoomsTotal <= 0)
        {
            return;
        }

        Corridor currentCorridor = StartingModule;
        Room currentRoom = null;

        Stack<Room> roomsStack = new Stack<Room>();
        Stack<Corridor> corridorStack = new Stack<Corridor>();

        while (roomsToCreate > 0)
        {
            //temporary generation algorithm
            bool newRoomCreated = false;
            int roomAttachAttempts = 0;
            int corridorAttachAttempts = 0;

            //Try to generate room by attaching to the current corridor
            foreach (LevelConnection corridorConnection in currentCorridor.connections)
            {
                //On each connection, try attaching a new room for maxAttempts
                roomAttachAttempts = 0;
                //a list of room modules that can attempt to be attached to a corridor, a copy of the public array at the start of each loop
                List<Room> roomModulePool = new List<Room>(roomModules);

                //If the connection is already used, go to next one
                if (corridorConnection.isAttached)
                {
                    continue;
                }

                //Grab a room module, and try attach a new room, if the pool still has something
                while (roomAttachAttempts < maxAttemptsAttachingRoom && (roomModulePool.Count > 0))
                {
                    Room room = GetRandomRoom(roomModulePool);
                    int newConnectionIndex = DetermineRoomConnectionCompatibility(room, corridorConnection);
                    bool newConnectionValidity = DetermineRoomLevelCompatibility(corridorConnection, newConnectionIndex, room);
                    if (newConnectionIndex >= 0 && newConnectionValidity)
                    {
                        //A new room can be attached to this corridor at this corridor connection
                        currentRoom = CreateRoom(corridorConnection, newConnectionIndex, room);
                        roomsToCreate -= 1;
                        newRoomCreated = true;
                        break;
                    }
                    roomAttachAttempts += 1;
                }

                //A new room is created, for now we only create one room at a time
                if (newRoomCreated)
                {
                    break;
                }
            }

            if (!newRoomCreated)
            {
                //If nothing can connect to this corridor, we retrace back to create/try another corridor
                //currentCorridor = corridorStack.Pop();
                break;
            }
            else
            {
                currentRoom.connections.Shuffle();
                //For each connection point in the current room, try attaching a corridor
                foreach (LevelConnection roomConnection in currentRoom.connections)
                {
                    //On each connection, try attaching a new corridor for maxAttempts
                    corridorAttachAttempts = 0;
                    //a list of corridor modules that can attempt to be attached to a room, a copy of the public array at the start of each loop
                    List<Corridor> corridorModulePool = new List<Corridor>(corridorModules);

                    //If connection is already used, go to next one
                    if (roomConnection.isAttached)
                    {
                        continue;
                    }

                    //Try attaching a new corridor to this connection, if the pool still has something
                    while (corridorAttachAttempts < maxAttemptsAttachingCorridor && (corridorModulePool.Count > 0))
                    {
                        Corridor corridor = GetRandomCorridor(corridorModulePool);

                        int newConnectionIndex = DetermineCorridorConnectionCompatibility(corridor, roomConnection);
                        bool newConnectionValidity = DetermineCorridorLevelCompatibility(roomConnection, newConnectionIndex, corridor);
                        if (newConnectionIndex >= 0 && newConnectionValidity)
                        {
                            //A new corridor can be attached to this room at this room connection
                            currentCorridor = CreateCorridor(roomConnection, newConnectionIndex, corridor);
                            break;
                        }
                        corridorAttachAttempts += 1;
                    }
                }
                //currentCorridor = ChooseNextCorridor(currentRoom);
            }
        }
    }

    /// <summary>
    /// Determines if the supplied room has the appropriate connections to attach to the corridor
    /// returns the valid roomConnection index, or -1 otherwise
    /// </summary>
    /// <param name="room"></param>
    /// <param name="corridorConnection"></param>
    /// <returns></returns>
    private int DetermineRoomConnectionCompatibility(Room room, LevelConnection corridorConnection)
    {
        int index = 0;
        foreach (LevelConnection roomConnection in room.connections)
        {
            switch (corridorConnection.direction)
            {
                case ConnectionDirection.Above:
                    if (roomConnection.direction == ConnectionDirection.Below)
                        return index;
                    break;
                case ConnectionDirection.Below:
                    if (roomConnection.direction == ConnectionDirection.Above)
                        return index;
                    break;
                case ConnectionDirection.Left:
                    if (roomConnection.direction == ConnectionDirection.Right)
                        return index;
                    break;
                case ConnectionDirection.Right:
                    if (roomConnection.direction == ConnectionDirection.Left)
                        return index;
                    break;
            }
            index += 1;
        }
        return -1;
    }

    /// <summary>
    /// Determines if the supplied corridor has the appropriate connections to attach to the room
    /// returns the valid corridorConnection, or -1 otherwise
    /// </summary>
    /// <param name="room"></param>
    /// <param name="corridorConnection"></param>
    /// <returns></returns>
    private int DetermineCorridorConnectionCompatibility(Corridor corridor, LevelConnection roomConnection)
    {
        int index = 0;
        foreach (LevelConnection corridorConnection in corridor.connections)
        {
            switch (roomConnection.direction)
            {
                case ConnectionDirection.Above:
                    if (corridorConnection.direction == ConnectionDirection.Below)
                        return index;
                    break;
                case ConnectionDirection.Below:
                    if (corridorConnection.direction == ConnectionDirection.Above)
                        return index;
                    break;
                case ConnectionDirection.Left:
                    if (corridorConnection.direction == ConnectionDirection.Right)
                        return index;
                    break;
                case ConnectionDirection.Right:
                    if (corridorConnection.direction == ConnectionDirection.Left)
                        return index;
                    break;
            }
            index += 1;
        }
        return -1;
    }

    /// <summary>
    /// Checks if the room about to be generated with overlap with the currently generated level
    /// Not yet implemented
    /// </summary>
    /// <returns></returns>
    private bool DetermineRoomLevelCompatibility(LevelConnection corridorConnection, int roomConnectionIndex, Room room)
    {
        if (roomConnectionIndex < 0)
        {
            //If the index is less than 0 that means the corridor connection is invalid, so return early
            return false;
        }

        Vector3 position = corridorConnection.transform.position;
        Vector3 roomOffset = room.connections[roomConnectionIndex].transform.position - room.transform.position;

        position -= roomOffset;

        Vector3 bottomLeft = room.RoomSize.bounds.min + position;
        Vector3 topRight = room.RoomSize.bounds.max + position;

        Collider2D[] collidersInsideOverlapArea = new Collider2D[1];

        int numberOfCollidersFound = Physics2D.OverlapAreaNonAlloc(bottomLeft, topRight, collidersInsideOverlapArea, LayerManager.LevelCollisionLayerMask);

        return (numberOfCollidersFound == 0);
    }

    /// <summary>
    /// Checks if the corridor about to be generated with overlap with the currently generated level
    /// Not yet implemented
    /// </summary>
    /// <returns></returns>
    private bool DetermineCorridorLevelCompatibility(LevelConnection roomConnection, int corridorConnectionIndex, Corridor corridor)
    {
        if (corridorConnectionIndex < 0)
        {
            //If the index is less than 0 that means the corridor connection is invalid, so return early
            return false;
        }

        Vector3 position = roomConnection.transform.position;
        Vector3 roomOffset = corridor.connections[corridorConnectionIndex].transform.position - corridor.transform.position;

        position -= roomOffset;

        Vector3 bottomLeft = corridor.RoomSize.bounds.min + position;
        Vector3 topRight = corridor.RoomSize.bounds.max + position;

        Collider2D[] collidersInsideOverlapArea = new Collider2D[1];

        int numberOfCollidersFound = Physics2D.OverlapAreaNonAlloc(bottomLeft, topRight, collidersInsideOverlapArea, LayerManager.LevelCollisionLayerMask);

        return (numberOfCollidersFound == 0);
    }

    /// <summary>
    /// Chooses the next corridor from which to try to generate a room for
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <returns></returns>
    private Corridor ChooseNextCorridor(Room currentRoom)
    {
        //For now, just choose the 2nd one
        return null;
    }

    /// <summary>
    /// Gets a random room from the pool, and remove it from the pool
    /// </summary>
    /// <param name="roomModulePool"></param>
    /// <returns></returns>
    private Room GetRandomRoom(List<Room> roomModulePool)
    {
        System.Random rnd = new System.Random();
        int index = rnd.Next(roomModulePool.Count);
        Room room = roomModulePool[index];
        roomModulePool.Remove(room);
        return room;
    }

    /// <summary>
    /// Gets a random corridor from the pool, and remove it from the pool
    /// </summary>
    /// <param name="corridorModulePool"></param>
    /// <returns></returns>
    private Corridor GetRandomCorridor(List<Corridor> corridorModulePool)
    {
        System.Random rnd = new System.Random();
        int index = rnd.Next(corridorModulePool.Count);
        Corridor corridor = corridorModulePool[index];
        corridorModulePool.Remove(corridor);
        return corridor;
    }

    /// <summary>
    /// Generates the specified room at the corridor location, with the specified roomConneciton
    /// </summary>
    /// <param name="corridorConnection"></param>
    /// <param name="roomConnection"></param>
    /// <param name="room"></param>
    Room CreateRoom(LevelConnection corridorConnection, int roomConnectionIndex, Room room)
    {
        Vector3 position = corridorConnection.transform.position;
        Vector3 roomOffset = room.connections[roomConnectionIndex].transform.position - room.transform.position;

        Room newRoom = Instantiate(room, position - roomOffset, Quaternion.identity, this.transform);
        corridorConnection.isAttached = true;
        newRoom.connections[roomConnectionIndex].isAttached = true;
        return newRoom;
    }

    /// <summary>
    /// Generates the specified corridor at the room location, with the specified corridorConnection
    /// </summary>
    /// <param name="corridorConnection"></param>
    /// <param name="roomConnection"></param>
    /// <param name="corridor"></param>
    Corridor CreateCorridor(LevelConnection roomConnection, int corridorConnectionIndex, Corridor corridor)
    {
        Vector3 position = roomConnection.transform.position;
        Vector3 corridorOffset = corridor.connections[corridorConnectionIndex].transform.position - corridor.transform.position;

        Corridor newCorridor = Instantiate(corridor, position - corridorOffset, Quaternion.identity, this.transform);
        newCorridor.connections[corridorConnectionIndex].isAttached = true;
        roomConnection.isAttached = true;
        return newCorridor;
    }
}
