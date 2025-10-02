using UnityEngine;
using System.Collections.Generic;

public class GroundPrefabSpawner : MonoBehaviour
{
    [Header("Grounds to Spawn On")]
    public List<GameObject> groundObjects; // Ground meshes, cubes, or terrains

    [Header("Prefabs to Spawn")]
    public List<GameObject> prefabsToSpawn; // The prefabs you want to spawn

    [Header("Spawner Settings")]
    [Range(0.1f, 10f)] public float density = 1f; // Higher = more prefabs
    public int maxObjects = 100; // Maximum number of prefabs to spawn
    public float yOffset = 0.1f; // Small offset above ground

    void Start()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        if (groundObjects.Count == 0 || prefabsToSpawn.Count == 0) return;

        int totalSpawned = 0;

        foreach (GameObject ground in groundObjects)
        {
            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer == null) continue;

            Vector3 size = renderer.bounds.size;
            Vector3 min = renderer.bounds.min;
            Vector3 max = renderer.bounds.max;

            int spawnCount = Mathf.RoundToInt(size.x * size.z * density);
            spawnCount = Mathf.Min(spawnCount, maxObjects - totalSpawned);

            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(min.x, max.x),
                    max.y + 10f, // start above ground for raycast
                    Random.Range(min.z, max.z)
                );

                // Raycast down to find ground surface
                if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, size.y + 20f))
                {
                    if (hit.collider.gameObject == ground)
                    {
                        GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
                        Vector3 spawnPos = hit.point + Vector3.up * yOffset;
                        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                        Instantiate(prefab, spawnPos, rotation, transform);
                        totalSpawned++;

                        if (totalSpawned >= maxObjects) return;
                    }
                }
            }
        }
    }
}
