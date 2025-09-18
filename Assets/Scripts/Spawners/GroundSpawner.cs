using UnityEngine;
using System.Collections.Generic;

public class GroundSpawner : MonoBehaviour
{
    [Header("Grounds to Spawn On")]
    public List<GameObject> groundObjects;

    [Header("Prefabs to Spawn (GameObjects)")]
    public List<GameObject> prefabsToSpawn;

    [Header("Meshes to Spawn (Optimized for Grass)")]
    public List<Mesh> meshesToSpawn;
    public Material grassMaterial;

    [Header("Spawner Settings")]
    [Range(0.1f, 10f)] public float density = 1f;
    public int maxObjects = 1000;
    public float yOffset = 0.1f;

    // Store transforms for mesh instances
    private Dictionary<Mesh, List<Matrix4x4>> meshInstances = new Dictionary<Mesh, List<Matrix4x4>>();

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        // Draw all grass meshes each frame
        foreach (var kvp in meshInstances)
        {
            Mesh mesh = kvp.Key;
            List<Matrix4x4> matrices = kvp.Value;

            // Unity can only draw 1023 at once
            for (int i = 0; i < matrices.Count; i += 1023)
            {
                int count = Mathf.Min(1023, matrices.Count - i);
                Graphics.DrawMeshInstanced(mesh, 0, grassMaterial, matrices.GetRange(i, count));
            }
        }
    }

    void Spawn()
    {
        if (groundObjects.Count == 0) return;

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
                    max.y + 10f,
                    Random.Range(min.z, max.z)
                );

                if (Physics.Raycast(randomPos, Vector3.down, out RaycastHit hit, size.y + 20f))
                {
                    if (hit.collider.gameObject == ground)
                    {
                        Vector3 spawnPos = hit.point + Vector3.up * yOffset;
                        Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                        Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);

                        if (meshesToSpawn.Count > 0 && Random.value < 0.7f) // 70% chance grass
                        {
                            Mesh mesh = meshesToSpawn[Random.Range(0, meshesToSpawn.Count)];
                            Matrix4x4 matrix = Matrix4x4.TRS(spawnPos, rotation, scale);

                            if (!meshInstances.ContainsKey(mesh))
                                meshInstances[mesh] = new List<Matrix4x4>();

                            meshInstances[mesh].Add(matrix);
                        }
                        else if (prefabsToSpawn.Count > 0) // fallback to prefab
                        {
                            GameObject prefab = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Count)];
                            Instantiate(prefab, spawnPos, rotation, transform);
                        }

                        totalSpawned++;
                        if (totalSpawned >= maxObjects) return;
                    }
                }
            }
        }
    }
}
