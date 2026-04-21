using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    [Header("Chunk Settings")]
    [Tooltip("Place your generated Ground/Chunk prefabs here.")]
    public GameObject[] chunkPrefabs;
    
    [Tooltip("The exact physical length of your chunks along the Z axis.")]
    public float chunkLength = 50f;
    
    [Tooltip("Number of chunks that should always exist concurrently ahead of the player.")]
    public int chunksOnScreen = 5;

    [Header("Player Tracking")]
    [Tooltip("The player's Transform. If left empty, assumes the player remains at Z = 0.")]
    public Transform playerTransform;

    // Track the Z coordinate for the NEXT chunk spawn
    private float nextSpawnZ = 0f;
    
    // We maintain a list to know exactly where the physical end of the track is at any given frame.
    private List<GameObject> activeChunks = new List<GameObject>();

    private void Start()
    {
        // At the start, pre-load our initial chunks so the world feels full immediately.
        // Often, we want the first chunk to begin slightly behind the player so they don't fall off.
        nextSpawnZ = -10f; 

        for (int i = 0; i < chunksOnScreen; i++)
        {
            SpawnChunk();
        }
    }

    private void Update()
    {
        // 1. Maintain tracking list: clean out any chunks that have destroyed themselves
        if (activeChunks.Count > 0 && activeChunks[0] == null)
        {
            activeChunks.RemoveAt(0);
        }

        // Determine target position securely
        float currentTargetZ = (playerTransform != null) ? playerTransform.position.z : 0f;

        // 2. Recalculate nextSpawnZ dynamically from the LAST active chunk's physical position.
        // EXPERT TIP: Why do we do this? In a moving world architecture, floating point errors
        // from Time.deltaTime accumulate. Recalculating from the actual physical Transform ensures
        // there are absolutely ZERO gaps forming between your chunks over time!
        if (activeChunks.Count > 0)
        {
            GameObject lastPhysicalChunk = activeChunks[activeChunks.Count - 1];
            if (lastPhysicalChunk != null)
            {
                nextSpawnZ = lastPhysicalChunk.transform.position.z + chunkLength;
            }
        }

        // 3. Check if the distance to the 'spawn horizon' requires a new chunk.
        // Using a while-loop instead of an if-statement makes this robust to lag spikes 
        // (if the game lags and moves 2 chunks' worth of space, it securely spawns 2).
        while (nextSpawnZ - currentTargetZ < (chunksOnScreen * chunkLength))
        {
            SpawnChunk();
        }
    }

    /// <summary>
    /// Selects a random prefab, instantiates it, and tracks its location.
    /// </summary>
    private void SpawnChunk()
    {
        if (chunkPrefabs == null || chunkPrefabs.Length == 0)
        {
            Debug.LogWarning("No chunk prefabs assigned to ChunkSpawner!");
            return;
        }

        // Pick a random array index
        int randomIndex = Random.Range(0, chunkPrefabs.Length);
        
        // Spawn strictly aligned forward, snapping it to the nextSpawnZ position
        GameObject newChunk = Instantiate(chunkPrefabs[randomIndex], new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        
        // Add to our list so the Update method can physically track its actual movement next frame
        activeChunks.Add(newChunk);
        
        // Advance the tracking marker for the immediate next chunk in the loop
        nextSpawnZ += chunkLength;
    }
}
