using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] float spawnTimer;
    float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            SpawnEnemy();
            timer = spawnTimer;
        }
    }

    private void SpawnEnemy()
    {
        Vector3 position = GenerateRandomPosition();
        position += PlayerStats.instance.transform.position; // Relative to Player

        GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity);
        
        newEnemy.transform.parent = transform; // Organize spawns into ENEMIES GameObject
    }

    private Vector3 GenerateRandomPosition()
    {
        Vector3 position = new Vector3();
        float sign = UnityEngine.Random.value > 0.5f ? -1f : 1f;

        if (UnityEngine.Random.value > 0.5f)
        {
            position.x = UnityEngine.Random.Range(-spawnArea.x, spawnArea.x);
            position.y = spawnArea.y * sign;
        }
        else
        {
            position.y = UnityEngine.Random.Range(-spawnArea.y, spawnArea.y);
            position.x = spawnArea.x * sign;
        }
        position.z = 0;

        return position;
    }
}
