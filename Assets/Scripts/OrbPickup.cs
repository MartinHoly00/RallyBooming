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
        inGameSystem = FindFirstObjectByType<InGameSystem>();
        orbSpawner = FindFirstObjectByType<OrbSpawner>();
        generateUpgrades = FindFirstObjectByType<GenerateUpgrades>();
        pauseSystem = FindFirstObjectByType<PauseSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelSystem = other.GetComponent<LevelSystem>();

            levelSystem.orbsCollected += 1;
            levelSystem.currentXP += levelSystem.xpPerOrb;
            PlayerPrefs.SetInt("XPCollected", PlayerPrefs.GetInt("XPCollected", 0) + 1);

            inGameSystem.UpdateXPUI(levelSystem.currentXP, levelSystem.xpTrashold, levelSystem.currentLevel);

            if (levelSystem.currentXP >= levelSystem.xpTrashold)
            {
                levelSystem.currentLevel += 1;
                levelSystem.currentXP = 0;
                levelSystem.xpTrashold += 10;
                levelSystem.xpPerOrb += 1;
                inGameSystem.UpdateXPUI(levelSystem.currentXP, levelSystem.xpTrashold, levelSystem.currentLevel);
                generateUpgrades.ShowUpgrades();
                pauseSystem.PauseGame();

                // Use Debug.Log instead of Console.WriteLine in Unity
                Debug.Log(
                    "Level Up! Current Level: " + levelSystem.currentLevel +
                    " | Orbs Collected: " + levelSystem.orbsCollected +
                    " | XP Threshold: " + levelSystem.xpTrashold +
                    " | XP per Orb: " + levelSystem.xpPerOrb
                );
            }
            if (pickupEffect != null)
            {
                GameObject explosion = Instantiate(pickupEffect, transform.position, transform.rotation);
                float effectDuration = explosion.GetComponent<ParticleSystem>().main.duration;
                Destroy(explosion, effectDuration);
            }
            Destroy(gameObject);
            orbSpawner.OrbCollected();
        }
    }
}