using UnityEngine;

public class OrbPickup : MonoBehaviour
{
    private LevelSystem levelSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            levelSystem = other.GetComponent<LevelSystem>();

            levelSystem.orbsCollected += 1;
            levelSystem.currentXP += levelSystem.xpPerOrb;

            if (levelSystem.currentXP >= levelSystem.xpTrashold)
            {
                levelSystem.currentLevel += 1;
                levelSystem.currentXP = 0;
                levelSystem.xpTrashold += 10;
                levelSystem.xpPerOrb += 1;

                // Use Debug.Log instead of Console.WriteLine in Unity
                Debug.Log(
                    "Level Up! Current Level: " + levelSystem.currentLevel +
                    " | Orbs Collected: " + levelSystem.orbsCollected +
                    " | XP Threshold: " + levelSystem.xpTrashold +
                    " | XP per Orb: " + levelSystem.xpPerOrb
                );
            }
            Destroy(gameObject);

        }
    }
}