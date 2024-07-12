using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // List of all unique enemy groups to be spawned in this wave
        public int waveQuota; // Minimum no. of enemies needed to be spawned in this wave
        public float spawnInterval; // Time interval between enemy spawn
        public int spawnCount; // Total no. of enemies already spawned in this wave
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public GameObject enemyPrefab;
        public int enemyCount; // Maximum no. of unique enemy to be spawned 
        public int spawnCount; // Total no. of unique enemy already spawned 
    }


    [Header("Spawner Attributes")]
    [SerializeField] Vector2 spawnArea;
    float spawnTimer; // Timer used to determine when to spawn the next enemy
    public int enemiesAlive; // Total no. of enemies existing on the map
    public int maxEnemiesAllowed; // Maximum no. of enemies allowed to exist at a time
    public bool maxEnemiesReached = false; // Flag to indicate maximum enemies existing


    [Header("Wave Attributes")]
    public int currentWave; // Index of the current wave (Starting from 0)
    public float waveInterval; // Time interval between waves
    bool isWaveActive = false; // Checks if a wave is currently ongoing
    public List<Wave> waves; // List of all waves in the game


    void Start()
    {
        CalculateWaveQuota();
    }

    private void Update()
    {
        // Checks if current wave has ended, starts next wave after wave interval
        if (currentWave < waves.Count && waves[currentWave].spawnCount == 0 && !isWaveActive)
        {
            isWaveActive = true;
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        // Spawn enemies until current wave quota is reached
        if (spawnTimer >= waves[currentWave].spawnInterval)
        {
            spawnTimer = 0f; // Reset timer
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);

        if (currentWave < waves.Count - 1) // Index starts from 0
        {
            isWaveActive = false; // Prevents multiple waves from starting simultaneously
            currentWave++; // Go through each wave
            CalculateWaveQuota();
        }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWave].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount; // Increase wave quota by the amount of enemies in the enemy group
        }

        waves[currentWave].waveQuota = currentWaveQuota; // Update the wave quota
    }

    // This method will stop spawning enemies if the amount of enemies existing on the map is maximum.
    // This method will only spawn enemies in a particular wave until it is time for the next wave enemies to be spawned.
    private void SpawnEnemies()
    {
        // If minimum amount of enemies to be spawned in the current wave not yet reached, keep spawning
        if (waves[currentWave].spawnCount < waves[currentWave].waveQuota && !maxEnemiesReached) 
        {
            // Spawn each type of enemy until wave quota filled
            foreach (var enemyGroup in waves[currentWave].enemyGroups)
            {
                // If minimum amount of unique enemy to be spawned is not yet reached, keep spawning
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    Vector3 spawnPosition = GenerateRandomPosition();
                    spawnPosition += PlayerStats.instance.transform.position; // Relative to Player

                    GameObject newEnemy = Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
                    newEnemy.transform.parent = transform; // Organize spawns into ENEMIES GameObject

                    enemyGroup.spawnCount++; // Increment unique enemy spawn count
                    waves[currentWave].spawnCount++; // Increment total enemy spawned in wave
                    enemiesAlive++; // Increment no. of enemies existing

                    // Limits the number of enemies that can exist at a time
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    public Vector3 GenerateRandomPosition()
    {
        Vector3 position = new Vector3();
        float sign = UnityEngine.Random.value > 0.5f ? -1f : 1f; // Randomly chooses + or -

        if (UnityEngine.Random.value > 0.5f) 
        {
            // Randomly spawns enemy along the top or bottom border
            position.x = UnityEngine.Random.Range(-spawnArea.x, spawnArea.x);
            position.y = spawnArea.y * sign;
        }
        else
        {
            // Randomly spawns enemy along the left or right border
            position.y = UnityEngine.Random.Range(-spawnArea.y, spawnArea.y);
            position.x = spawnArea.x * sign;
        }
        position.z = 0; // Vector3

        return position;
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;

        // Reset flag if no. of enemies dropped below maximum
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
