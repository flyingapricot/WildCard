using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject currentChunk;
    public float checkerRadius;
    public LayerMask terrainMask;
    private Vector3 previousPlayerPosition;


    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    public float maxOpDist; // Must be greater than the length and width of tilemap (50 > 20)
    float opDist;
    float opCooldown;
    public float opCooldownDur;


    void Start()
    {
        previousPlayerPosition = PlayerStats.instance.transform.position;

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

    string GetDirectionName(Vector3 direction)
    {
        direction = direction.normalized; // When normalized, a vector keeps the same direction but its length is 1.0.

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) // Horizontal movement
        {
            if (direction.y > 0.5f) // + Upwards movement
            {
                return direction.x > 0 ? "TopRight" : "TopLeft";
            }
            else if (direction.y < -0.5f) // + Downwards movement
            {
                return direction.x > 0 ? "BottomRight" : "BottomLeft";
            }
            else
            {
                return direction.x > 0 ? "Right" : "Left";
            }
        }
        else // Vertical movement
        {
            if (direction.x > 0.5f) // + Rightwards movement
            {
                return direction.y > 0 ? "TopRight" : "BottomRight";
            }
            else if (direction.x < -0.5f) // + Leftwards movement
            {
                return direction.y > 0 ? "TopLeft" : "BottomLeft";
            }
            else
            {
                return direction.y > 0 ? "Up" : "Down";
            }
        }
    }

    void ChunkChecker()
    {
        if (!currentChunk)
        {
            //Debug.LogWarning("Current chunk is null. Ensure a chunk is set or instantiated at the start.");
            return;
        }

        // Find what direction the player is moving in by comparing positions between frames
        Vector3 moveDir = PlayerStats.instance.transform.position - previousPlayerPosition;
        previousPlayerPosition = PlayerStats.instance.transform.position;

        string dirName = GetDirectionName(moveDir);

        Transform dir = currentChunk.transform.Find("Directions");
        if (dir == null)
        {
            Debug.LogError("Directions transform not found in current chunk. Ensure it exists and is named correctly.");
            return;
        }

        // Spawns 3 chunks in the direction of movement
        if (dirName == "Right") 
        {
            ChunkDir(dir, "Right", "TopRight", "BottomRight");
        }
        else if (dirName == "Left") 
        {
            ChunkDir(dir, "Left", "TopLeft", "BottomLeft");
        }
        else if (dirName == "Up") 
        {
            ChunkDir(dir, "Up", "TopRight", "TopLeft");
        }
        else if (dirName == "Down") 
        {
            ChunkDir(dir, "Down", "BottomRight", "BottomLeft");
        }
        else if (dirName == "TopRight") 
        {
            ChunkDir(dir, "TopRight", "Right", "Up");
        }
        else if (dirName == "BottomRight")
        {
            ChunkDir(dir, "BottomRight", "Right", "Down");
        }
        else if (dirName == "BottomLeft") 
        {
            ChunkDir(dir, "BottomLeft", "Left", "Down");
        }
        else if (dirName == "TopLeft")
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

    void ChunkSpawner(Transform dir, string dirName)
    {
        Transform targetTransform = dir.Find(dirName); // Get the chunk in this direction
        if (targetTransform == null)
        {
            Debug.LogError($"Direction {dirName} not found in Directions transform.");
            return;
        }

        // Checks if chunk is already spawned in direction.
        if (!Physics2D.OverlapCircle(targetTransform.position, checkerRadius, terrainMask))
        {
            //noTerrainPosition = targetTransform.position;
            SpawnChunk(targetTransform.position);
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rng = Random.Range(0, terrainChunks.Count); // Randomize which chunk to spawn
        GameObject newChunk = Instantiate(terrainChunks[rng], spawnPosition, Quaternion.identity);
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
