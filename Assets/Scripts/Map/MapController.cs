using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject currentChunk;
    public float checkerRadius;
    Vector3 noTerrainPosition;
    public LayerMask terrainMask;
    PlayerMovement pm;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    public float maxOpDist; // Must be greater than the length and width of tilemap (50 > 20)
    float opDist;
    float opCooldown;
    public float opCooldownDur;

    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();

        if (currentChunk == null && terrainChunks.Count > 0)
        {
            int rng = Random.Range(0, terrainChunks.Count); // Initialize a random chunk if none is set
            currentChunk = Instantiate(terrainChunks[rng], Vector3.zero, Quaternion.identity);
            currentChunk.transform.parent = transform; // Organize chunk into MapController GameObject
        }
    }

    void Update()
    {
        ChunkChecker();
        ChunkOptimizer();
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            //Debug.LogWarning("Current chunk is null. Ensure a chunk is set or instantiated at the start.");
            return;
        }

        Transform dir = currentChunk.transform.Find("Directions");
        if (dir == null)
        {
            Debug.LogError("Directions transform not found in current chunk. Ensure it exists and is named correctly.");
            return;
        }

        // Spawns 3 chunks in the direction of player
        if (pm.movementVector.x > 0 && pm.movementVector.y == 0) // Right
        {
            ChunkDir(dir, "Right", "TopRight", "BottomRight");
        }
        else if (pm.movementVector.x < 0 && pm.movementVector.y == 0) // Left
        {
            ChunkDir(dir, "Left", "TopLeft", "BottomLeft");
        }
        else if (pm.movementVector.x == 0 && pm.movementVector.y > 0) // Up
        {
            ChunkDir(dir, "Up", "TopRight", "TopLeft");
        }
        else if (pm.movementVector.x == 0 && pm.movementVector.y < 0) // Down
        {
            ChunkDir(dir, "Down", "BottomRight", "BottomLeft");
        }
        else if (pm.movementVector.x > 0 && pm.movementVector.y > 0) // Top Right
        {
            ChunkDir(dir, "TopRight", "Right", "Up");
        }
        else if (pm.movementVector.x > 0 && pm.movementVector.y < 0) // Bottom Right
        {
            ChunkDir(dir, "BottomRight", "Right", "Down");
        }
        else if (pm.movementVector.x < 0 && pm.movementVector.y < 0) // Bottom Left
        {
            ChunkDir(dir, "BottomLeft", "Left", "Down");
        }
        else if (pm.movementVector.x < 0 && pm.movementVector.y > 0) // Top Left
        {
            ChunkDir(dir, "TopLeft", "Left", "Up");
        }
    }
    
    void ChunkDir(Transform dir, string direction1, string direction2, string direction3)
    {
        ChunkSpawner(dir, direction1);
        ChunkSpawner(dir, direction2);
        ChunkSpawner(dir, direction3);
    }

    void ChunkSpawner(Transform dir, string direction)
    {
        Transform targetTransform = dir.Find(direction);
        if (targetTransform == null)
        {
            Debug.LogError($"Direction {direction} not found in Directions transform.");
            return;
        }

        if (!Physics2D.OverlapCircle(targetTransform.position, checkerRadius, terrainMask))
        {
            noTerrainPosition = targetTransform.position;
            SpawnChunk();
        }
    }

    void SpawnChunk()
    {
        int rng = Random.Range(0, terrainChunks.Count); // Randomize which chunk to spawn
        GameObject newChunk = Instantiate(terrainChunks[rng], noTerrainPosition, Quaternion.identity);
        currentChunk = newChunk; // Update current chunk
        //Debug.Log("Spawned new chunk at: " + noTerrainPosition);
        newChunk.transform.parent = transform; // Organize chunks into MapController GameObject
        spawnedChunks.Add(currentChunk);
    }

    void ChunkOptimizer()
    {
        opCooldown -= Time.deltaTime; // Prevent continuous checks
        if (opCooldown <= 0f)
        {
            opCooldown = opCooldownDur;
        }
        else
        {
            return;
        }

        foreach (GameObject chunk in spawnedChunks) // Unloads chunks far away from player (Don't destroy)
        {
            opDist = Vector3.Distance(PlayerStats.instance.transform.position, chunk.transform.position);
            if (opDist > maxOpDist)
            {
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
