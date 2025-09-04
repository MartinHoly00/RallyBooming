using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [Header("Orb Settings")]
    public GameObject orbPrefab;       // Orb prefab to spawn
    public int maxOrbs = 10;           // Maximum number of orbs at once
    public float spawnRadius = 20f;    // Radius around the car
    public float spawnInterval = 2f;   // Seconds between spawns

    [Header("References")]
    public Transform carTransform;     // Reference to the car

    private int currentOrbCount = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnOrb), 1f, spawnInterval);
    }

    void SpawnOrb()
    {
        if (currentOrbCount >= maxOrbs) return;

        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = new Vector3(
            randomCircle.x,
            carTransform.position.y + 0.5f,  // same height as car
            randomCircle.y
        );

        Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        currentOrbCount++;
    }

    public void OrbCollected()
    {
        currentOrbCount = Mathf.Max(0, currentOrbCount - 1);
    }
}
