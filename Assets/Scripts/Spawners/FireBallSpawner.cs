using UnityEngine;

public class FireBallSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject fireballPrefab;
    public GameObject ground;
    public float spawnHeight = 50f;
    public float initialSpawnInterval = 1f;
    public float minSpawnInterval = 0.1f;
    public float difficultyRampSpeed = 0.01f; // how much faster per second

    private float spawnRadius;
    private Vector3 groundCenter;
    private float currentSpawnInterval;
    private float timer;
    private InGameSystem inGameSystem;


    void Start()
    {
        CalculateSpawnArea();
        currentSpawnInterval = initialSpawnInterval;
        timer = currentSpawnInterval;
        inGameSystem = FindFirstObjectByType<InGameSystem>();
    }

    void Update()
    {
        HealthSystem hs = FindFirstObjectByType<HealthSystem>();
        if ((hs != null && hs.isDestroyed) || inGameSystem.isPaused) return;

        // Countdown timer
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnFireball();

            // Decrease interval (increase difficulty) but clamp to min
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - difficultyRampSpeed);

            // Reset timer
            timer = currentSpawnInterval;
        }
    }

    void CalculateSpawnArea()
    {
        if (ground != null)
        {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                Bounds bounds = groundRenderer.bounds;
                groundCenter = bounds.center;
                spawnRadius = Mathf.Max(bounds.size.x, bounds.size.z) * 0.4f;
            }
        }
    }

    void SpawnFireball()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = new Vector3(
            groundCenter.x + randomCircle.x,
            groundCenter.y + spawnHeight,
            groundCenter.z + randomCircle.y
        );

        Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);
    }
}
