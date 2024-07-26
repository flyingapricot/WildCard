using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySpawner;

public class SwarmEnemy : MonoBehaviour
{

    //Generating swarm of enemies
    [SerializeField] public int SwarmSize;
    [SerializeField] public GameObject enemyPrefab;

    void Start()
    {
        SpawnSwarm();
    }

    public void SpawnSwarm()
    {
        for (int i = 0; i < SwarmSize; i++)
        {
            Vector3 spawnPosition = PlayerStats.instance.transform.position;
            spawnPosition.x += 2;

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            //newEnemy.transform.parent = transform; // Organize spawns into ENEMIES GameObject
        }
    }




}
