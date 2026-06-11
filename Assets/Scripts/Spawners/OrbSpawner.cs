using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;
    public float maxOrbs = 10;
    public float spawnInterval = 2f;
    [Tooltip("How high above the ground surface the orb is placed.")]
    public float orbHeightAboveGround = 1f;
    [Tooltip("Keep spawns this far inside the edges of the ground area.")]
    public float spawnEdgeMargin = 50f;

    [Tooltip("How many spots to try per spawn before giving up (higher helps on crowded city maps).")]
    public int maxSpawnAttempts = 12;

    [Header("References")]
    public GameObject ground;
    [Tooltip("Which layers count as ground when placing orbs on the surface.")]
    public LayerMask groundMask = ~0;
    [Tooltip("Layers that block spawning (buildings, obstacles). Orbs won't spawn on or inside these.")]
    public LayerMask obstacleMask;

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

        if (orbPrefab.GetComponent<OrbPickup>())
        {
            LevelSystem levelSystem = FindFirstObjectByType<LevelSystem>();
            maxOrbs = levelSystem.maxOrbs;
        }

        InvokeRepeating(nameof(SpawnOrb), 1f, spawnInterval);
    }

    void SpawnOrb()
    {
        if (currentOrbCount >= maxOrbs) return;

        HealthSystem hs = FindFirstObjectByType<HealthSystem>();
        if (hs != null && hs.isDestroyed) return;

        // Cast against ground AND obstacles together, then look at what the ray
        // hits first. A building fills the whole column from street to roof, so
        // a ray dropped from the sky hits its roof first whenever that X/Z is
        // inside or under a building - letting us reject those spots and only
        // spawn on open ground. We retry a few times so crowded city maps still
        // find a clear spot rather than skipping the spawn.
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            float randomX = Random.Range(groundBounds.min.x + spawnEdgeMargin, groundBounds.max.x - spawnEdgeMargin);
            float randomZ = Random.Range(groundBounds.min.z + spawnEdgeMargin, groundBounds.max.z - spawnEdgeMargin);

            Vector3 rayStart = new Vector3(randomX, groundBounds.max.y + 50f, randomZ);
            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundMask | obstacleMask, QueryTriggerInteraction.Ignore))
                continue; // nothing under this point (gap in the map) - try elsewhere

            bool hitGround = (groundMask.value & (1 << hit.collider.gameObject.layer)) != 0;
            if (!hitGround)
                continue; // first thing hit was a building/obstacle - spot is occupied

            Vector3 spawnPos = hit.point + Vector3.up * orbHeightAboveGround;
            Instantiate(orbPrefab, spawnPos, Quaternion.identity);
            currentOrbCount++;
            return;
        }
        // Couldn't find a clear spot this interval; we'll try again next time.
    }

    public void OrbCollected()
    {
        currentOrbCount = Mathf.Max(0, currentOrbCount - 1);
    }
}
