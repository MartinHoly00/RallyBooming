using UnityEngine;

public class OrbPickup : MonoBehaviour
{
    public GameObject pickupEffect;
    private LevelSystem levelSystem;
    private InGameSystem inGameSystem;
    private OrbSpawner orbSpawner;
    private GenerateUpgrades generateUpgrades;
    private PauseSystem pauseSystem;

    private void Start()
    {
        // try to find scene singletons / managers (may return null if not present)
        inGameSystem = FindFirstObjectByType<InGameSystem>();
        orbSpawner = FindFirstObjectByType<OrbSpawner>();
        generateUpgrades = FindFirstObjectByType<GenerateUpgrades>();
        pauseSystem = FindFirstObjectByType<PauseSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // try multiple ways to get LevelSystem from the player collider
        levelSystem = other.GetComponent<LevelSystem>()
                      ?? other.GetComponentInParent<LevelSystem>()
                      ?? other.GetComponentInChildren<LevelSystem>()
                      ?? FindFirstObjectByType<LevelSystem>();

        if (levelSystem == null)
        {
            Debug.LogError($"OrbPickup: No LevelSystem found on player object '{other.gameObject.name}'. " +
                           "Attach LevelSystem to the Player or its parents/children, or adjust retrieval logic.");
            return; // safety: stop processing if we don't have level data
        }

        // update XP / counters
        levelSystem.orbsCollected += 1;
        levelSystem.currentXP += levelSystem.xpPerOrb;
        PlayerPrefs.SetInt("XPCollected", PlayerPrefs.GetInt("XPCollected", 0) + 1);

        // update UI if available
        if (inGameSystem != null)
        {
            inGameSystem.UpdateXPUI(levelSystem.currentXP, levelSystem.xpTrashold, levelSystem.currentLevel);
        }
        else
        {
            Debug.LogWarning("OrbPickup: inGameSystem is null — cannot UpdateXPUI.");
        }

        // level up logic
        if (levelSystem.currentXP >= levelSystem.xpTrashold)
        {
            levelSystem.currentLevel += 1;
            levelSystem.currentXP = 0;
            levelSystem.xpTrashold += 10;
            levelSystem.xpPerOrb += 1;

            if (inGameSystem != null)
                inGameSystem.UpdateXPUI(levelSystem.currentXP, levelSystem.xpTrashold, levelSystem.currentLevel);

            if (generateUpgrades != null) generateUpgrades.ShowUpgrades();
            else Debug.LogWarning("OrbPickup: generateUpgrades is null — cannot ShowUpgrades.");

            if (pauseSystem != null) pauseSystem.PauseGame();
            else Debug.LogWarning("OrbPickup: pauseSystem is null — cannot PauseGame.");

            Debug.Log($"Level Up! Level: {levelSystem.currentLevel} | Orbs: {levelSystem.orbsCollected} | Threshold: {levelSystem.xpTrashold}");
        }

        // spawn effect safely
        if (pickupEffect != null)
        {
            GameObject explosion = Instantiate(pickupEffect, transform.position, transform.rotation);

            // try to determine particle duration safely
            ParticleSystem ps = explosion.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                // compute an approximate lifetime: duration + startLifetime (only when meaningful)
                var main = ps.main;
                float life = main.duration;
                // add startLifetime if it's a constant (best-effort)
                if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                    life += main.startLifetime.constant;
                Destroy(explosion, Mathf.Max(0.5f, life));
            }
            else
            {
                // fallback
                Destroy(explosion, 3f);
            }
        }

        // remove orb and notify spawner (if present)
        Destroy(gameObject);

        if (orbSpawner != null)
        {
            orbSpawner.OrbCollected();
        }
        else
        {
            Debug.LogWarning("OrbPickup: orbSpawner is null — OrbCollected() not called.");
        }
    }
}
