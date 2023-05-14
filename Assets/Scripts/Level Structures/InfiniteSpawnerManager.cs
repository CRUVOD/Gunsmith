using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to handle inifnite spawners in rooms, add this on top of room scripts
/// </summary>
public class InfiniteSpawnerManager : MonoBehaviour
{
    //How the infinite spawner will stop, by running out time or killing one specific enemy
    public enum InfniteSpawnerStoppingMethod { Time, Enemy}
    public InfniteSpawnerStoppingMethod stoppingMethod;

    [Header("Time")]
    public float timeTillStop;

    [Header("Enemy")]
    public Enemy deciderEnemy;

    public InfiniteEnemySpawner[] infiniteEnemySpawners;

    private void Start()
    {
        if (stoppingMethod == InfniteSpawnerStoppingMethod.Enemy)
        {

        }
    }

    void OnDeciderEnemyDeath(Enemy enemy)
    {
        StopAllSpawners();
    }

    private void StopAllSpawners()
    {
        for (int i = 0; i < infiniteEnemySpawners.Length; i++)
        {

        }
    }
}
