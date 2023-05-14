using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteEnemySpawner : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;

    public List<Enemy> enemiesCanSpawn;

    //On spawn delegate 
    public EnemySpawner.OnSpawnDelegate OnSpawn;
    public EnemySpawner.OnFinishDelegate OnFinish;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SpawnRandomEnemy()
    {
        int index = Random.Range(0, enemiesCanSpawn.Count-1);
        StartCoroutine(SpawnEnemy(enemiesCanSpawn[index]));
    }

    /// <summary>
    /// Go through the list of enemies to spawn, spawn enemies one by one with specified delay
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnEnemy(Enemy enemy)
    {
        animator.SetBool("activated", true);

        Enemy spawnedEnemy = Instantiate(enemy, this.gameObject.transform.position, Quaternion.identity);
        //On spawn event
        OnSpawn?.Invoke(spawnedEnemy);
        yield return new WaitForSeconds(0.5f);

        animator.SetBool("activated", false);
    }


}
