using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCubeSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints; // Assign your 5 points in the Inspector
    public GameObject fallingCubePrefab;

    [Header("Spawn Settings")]
    public float minSpawnDelay = 0.75f; // Reduced for faster spawning
    public float maxSpawnDelay = 1.5f;
    public float cubeFallSpeed = 15f; // Increased fall speed
    public float spawnHeightOffset = 5f; // Spawn cubes higher up
    public float despawnHeight = -15f; // Lower despawn since cubes start higher


    [Header("Cube Settings")]
    public float cubeLifetime = 8f; // Max time before auto-despawning
    public Material normalMaterial;
    public Material frozenMaterial;

    [Header("Sequential Spawning")]
    public bool spawnInOrder = true; // Toggle between sequential and random
    private int currentSpawnIndex = 0;

    private List<GameObject> activeCubes = new List<GameObject>();

    void Start()
    {
        // Start spawning cubes
        StartCoroutine(SpawnCubesRoutine());

        // Start cleanup check
        StartCoroutine(CleanupCubesRoutine());
    }

    IEnumerator SpawnCubesRoutine()
    {
        while (true)
        {
            // Wait random time between spawns (shorter delays for faster spawning)
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            // Spawn a cube
            if (spawnInOrder)
            {
                SpawnCubeInSequence();
            }
           
        }
    }


    void SpawnCubeInSequence()
    {
        // Get current spawn point in sequence
        Transform spawnPoint = spawnPoints[currentSpawnIndex];

        // Calculate spawn position (higher up)
        Vector3 spawnPosition = spawnPoint.position + Vector3.up * spawnHeightOffset;

        // Spawn the cube
        SpawnCubeAtPosition(spawnPosition);

        // Move to next spawn point in sequence
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;

        Debug.Log($"Spawned cube at point {currentSpawnIndex + 1}");
    }

    void SpawnCubeAtPosition(Vector3 position)
    {
        GameObject cube = Instantiate(fallingCubePrefab, position, Quaternion.identity);

        // Add falling behavior
        FallingCube cubeScript = cube.GetComponent<FallingCube>();
        if (cubeScript == null)
        {
            cubeScript = cube.AddComponent<FallingCube>();
        }

        // Initialize cube with faster falling speed
        cubeScript.Initialize(cubeFallSpeed, despawnHeight, cubeLifetime, normalMaterial, frozenMaterial);

        activeCubes.Add(cube);
    }

    public void SetSpawnOrder(bool useSequential)
    {
        spawnInOrder = useSequential;
        currentSpawnIndex = 0; // Reset to start from point 1
    }

    IEnumerator CleanupCubesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            // Remove null references and destroyed cubes
            activeCubes.RemoveAll(cube => cube == null);
        }
    }

    // Optional: Draw spawn points in editor for easy setup
    void OnDrawGizmos()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireCube(point.position, Vector3.one * 0.5f);
                    Gizmos.DrawLine(point.position, point.position + Vector3.down * 2f);
                }
            }
        }
    }
}
