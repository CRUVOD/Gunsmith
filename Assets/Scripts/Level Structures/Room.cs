using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the events inside this combat room
/// </summary>
public class Room : MonoBehaviour
{
    [Header("Level And Room")]
    public BoxCollider2D RoomSize;
    public LevelConnection[] connections;
    public RoomPlayerDetector playerDetector;

    [Header("Enemy Tracking")]
    //These two lists keep track of the spawners and enemies currently present in the room
    public List<Enemy> enemiesInRoom;
    public List<EnemySpawner> spawnersInRoom;

    /// <summary>
    /// Dictates when should the the spawners in the room activate, whether by a fxied time, or if a certain number of enemies in the room is left
    /// </summary>
    public enum SpawnerMethod { TimeCountdown, EnemyInRoom}

    [Header("Spawner settings")]
    public SpawnerMethod spawnerMethod;
    [Tooltip("The amount of time after when player enter this room that the spawners will start")]
    public float timeTillSpawn;
    float timeTillSpawnTimer;
    [Tooltip("If total enemies in room equals or is lower than this number, start spawning enemies")]
    public int enemyCountThreshold;

    bool playerIsInRoom;
    bool spawnersStarted;

    private void Start()
    {
        //Listens to the playerdetector for events
        playerDetector.OnPlayerEnter += PlayerEnterHandler;
        playerDetector.OnPlayerExit += PlayerExitHandler;

        //Listens to every enemy that are in the room already for their death delegate
        for (int i = 0; i < enemiesInRoom.Count; i++)
        {
            enemiesInRoom[i].OnDeath += EnemyOnDeathHandler;
        }

        for (int i = 0; i < spawnersInRoom.Count; i++)
        {
            spawnersInRoom[i].OnSpawn += SpawnerOnSpawnHandler;
            spawnersInRoom[i].OnFinish += SpawnerOnFinishHandler;
        }

        timeTillSpawnTimer = timeTillSpawn;
        playerIsInRoom = false;
        spawnersStarted = false;

        WaitForPlayer();
    }

    /// <summary>
    /// Waits for the player to enter the room's boundaries, thus
    /// de-activating the enemies and spawners in the room for now
    /// </summary>
    private void WaitForPlayer()
    {
        for (int i = 0; i < enemiesInRoom.Count; i++)
        {
            enemiesInRoom[i].ActivateEnemy(false);
        }

        for (int i = 0; i < spawnersInRoom.Count; i++)
        {
            spawnersInRoom[i].gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!spawnersStarted)
        {
            //Handle spawners if they haven't started spawning
            HandleSpawners();
        }
    }

    private void HandleSpawners()
    {
        if (!playerIsInRoom)
        {
            return;
        }

        if (spawnerMethod == SpawnerMethod.TimeCountdown)
        {
            if (timeTillSpawnTimer > 0)
            {
                timeTillSpawnTimer -= Time.deltaTime;
            }
            else
            {
                StartAllSpawners();
            }
        }
        else if (spawnerMethod == SpawnerMethod.EnemyInRoom)
        {
            if (enemiesInRoom.Count <= enemyCountThreshold)
            {
                StartAllSpawners();
            }
        }
    }

    private void StartAllSpawners()
    {
        for (int i = 0; i < spawnersInRoom.Count; i++)
        {
            spawnersInRoom[i].StartSpawningEnemies();
            spawnersStarted = true;
        }
    }

    private void PauseAllSpawners()
    {
        for (int i = 0; i < spawnersInRoom.Count; i++)
        {
            spawnersInRoom[i].PauseEnemySpawning();
        }
    }

    /// <summary>
    /// If both enemies in the room and the spawners are all dead/finished/cleared.
    /// Release the player from the room
    /// </summary>
    private void CheckRoomClear()
    {
        if (IsRoomClear())
        {
            LevelManager.instance.ActivateDoors(false);
        }
    }

    /// <summary>
    /// Returns true if no enemies or spawners are left in the room
    /// </summary>
    /// <returns></returns>
    private bool IsRoomClear()
    {
        if (spawnersInRoom.Count == 0 && enemiesInRoom.Count == 0)
        {
            return true;
        }
        return false;
    }

    #region EventHandlers

    private void PlayerEnterHandler()
    {
        playerIsInRoom = true;
        if (!IsRoomClear())
        {
            LevelManager.instance.ActivateDoors(true);
            for (int i = 0; i < enemiesInRoom.Count; i++)
            {
                enemiesInRoom[i].ActivateEnemy(true);
            }

            for (int i = 0; i < spawnersInRoom.Count; i++)
            {
                spawnersInRoom[i].gameObject.SetActive(true);
            }
        }
    }

    private void PlayerExitHandler()
    {
        playerIsInRoom = false;
        //for (int i = 0; i < enemiesInRoom.Count; i++)
        //{
        //    enemiesInRoom[i].gameObject.SetActive(false);
        //}

        //for (int i = 0; i < spawnersInRoom.Count; i++)
        //{
        //    spawnersInRoom[i].gameObject.SetActive(false);
        //}
    }

    private void EnemyOnDeathHandler(Enemy deadEnemy)
    {
        enemiesInRoom.Remove(deadEnemy);
        CheckRoomClear();
    }

    private void SpawnerOnSpawnHandler(Enemy newEnemy)
    {
        enemiesInRoom.Add(newEnemy);
        newEnemy.OnDeath += EnemyOnDeathHandler;
    }

    private void SpawnerOnFinishHandler(EnemySpawner spawner)
    {
        spawnersInRoom.Remove(spawner);
        CheckRoomClear();
    }

    #endregion

}
