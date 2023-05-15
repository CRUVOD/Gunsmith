using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Simple class to store what type of enemy to spawn, and how many should be spawned
[Serializable]
public class SpawnSet
{
    public Enemy Enemy;
    public int Number;
}

public class EnemySpawner : MonoBehaviour
{
    public Animator animator;

    public List<SpawnSet> spawnSets;

    protected Queue<Enemy> enemiesToSpawn;

    [HideInInspector]
    public bool isSpawningEnemies;

    //On spawn delegate 
    public delegate void OnSpawnDelegate(Enemy enemySpawned);
    public OnSpawnDelegate OnSpawn;

    //Spawn finished delegate
    public delegate void OnFinishDelegate(EnemySpawner spawner);
    public OnFinishDelegate OnFinish;

    //the delay between spawning each enemy in the spawnsets
    public float timeBetweenEachEnemySpawned;

    //number of enemies in total in the spawner
    int totalEnemiesToSpawn;

    private void Start()
    {
        enemiesToSpawn = new Queue<Enemy>();

        for (int i = 0; i < spawnSets.Count; i++)
        {
            totalEnemiesToSpawn += spawnSets[i].Number;
        }
        isSpawningEnemies = false;
        GenerateSpawnOrder();
    }

    /// <summary>
    /// Add a new spawnset to this spawner
    /// </summary>
    /// <param name="newSet"></param>
    public void AddSpawnSet(SpawnSet newSet)
    {
        spawnSets.Add(newSet);
        GenerateSpawnOrder();
    }

    /// <summary>
    /// Create a linear list for the OnSpawn function to cycle through, spawning each enemy as needed
    /// Always erases the previous spawn order
    /// </summary>
    private void GenerateSpawnOrder()
    {
        enemiesToSpawn.Clear();
        List<Enemy> tempList = new List<Enemy>();

        for (int i = 0; i < spawnSets.Count; i++)
        {
            for (int j = 0; j < spawnSets[i].Number; j++)
            {
                tempList.Add(spawnSets[i].Enemy);
            }
        }

        //Shuffles the spawn order
        tempList.Shuffle();

        //Add the enemies to the queue in the shuffled ordeer
        for (int i = 0; i < tempList.Count; i++)
        {
            enemiesToSpawn.Enqueue(tempList[i]);
        }
    }

    public void StartSpawningEnemies()
    {
        animator.SetBool("activated", true);
        if (!isSpawningEnemies && enemiesToSpawn.Count > 0)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    public void PauseEnemySpawning()
    {
        StopAllCoroutines();
        isSpawningEnemies = false;
    }

    /// <summary>
    /// Go through the list of enemies to spawn, spawn enemies one by one with specified delay
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnEnemies()
    {
        isSpawningEnemies = true;
        while(enemiesToSpawn.Count != 0)
        {
            Enemy spawnedEnemy = Instantiate(enemiesToSpawn.Dequeue(), this.gameObject.transform.position, Quaternion.identity);
            //On spawn event
            OnSpawn?.Invoke(spawnedEnemy);
            spawnedEnemy.ActivateEnemy(true);
            yield return new WaitForSeconds(timeBetweenEachEnemySpawned);
        }
        isSpawningEnemies = false;

        if (enemiesToSpawn.Count <= 0)
        {
            SpawningFinished();
        }
    }

    private void SpawningFinished()
    {
        animator.SetBool("activated", false);
        //On finish event
        OnFinish?.Invoke(this);
        //Small delay before destroy to make sure stuff is properly exiting before disappearing
        Destroy(this.gameObject, .1f);
    }
}
