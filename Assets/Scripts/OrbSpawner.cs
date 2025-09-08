using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;
    public int maxOrbs = 10;
    public float spawnInterval = 2f;
    [Header("References")]
    public GameObject ground;

    private int currentOrbCount = 0;
    private Bounds groundBounds;

    void Start()
    {
        if (ground != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundBounds = groundRenderer.bounds;
            }
        }

        InvokeRepeating(nameof(SpawnOrb), 1f, spawnInterval);
    }

    void SpawnOrb()
    {
        if (currentOrbCount >= maxOrbs) return;

        HealthSystem hs = FindFirstObjectByType<HealthSystem>();
        if (hs != null && hs.isDestroyed) return;

        float randomX = Random.Range(groundBounds.min.x + 50f, groundBounds.max.x - 50f);
        float randomZ = Random.Range(groundBounds.min.z + 50f, groundBounds.max.z - 50f);

        Vector3 spawnPos = new Vector3(randomX, groundBounds.max.y + 1f, randomZ);

        Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        currentOrbCount++;
    }

    public void OrbCollected()
    {
        currentOrbCount = Mathf.Max(0, currentOrbCount - 1);
    }
}
