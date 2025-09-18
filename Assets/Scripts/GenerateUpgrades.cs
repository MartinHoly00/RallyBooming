using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateUpgrades : MonoBehaviour
{
    public int numberOfUpgrades = 3;
    public GameObject upgradePanel;
    public GameObject upgradePrefab;
    public GameObject xpSpawnerObject;
    public UpgradeDatabase upgradeDatabase;

    private UpgradeData[] selectedUpgrades;

    private InGameSystem inGameSystem;
    private PauseSystem pauseSystem;
    private CarControl carControl;
    private LevelSystem levelSystem;
    private OrbSpawner xpSpawner;
    private HealthSystem healthSystem;
    private ScoreSystem scoreSystem;

    void Start()
    {
        inGameSystem = FindFirstObjectByType<InGameSystem>();
        pauseSystem = FindFirstObjectByType<PauseSystem>();
        carControl = FindAnyObjectByType<CarControl>();
        levelSystem = FindFirstObjectByType<LevelSystem>();
        xpSpawner = xpSpawnerObject.GetComponent<OrbSpawner>();
        healthSystem = FindFirstObjectByType<HealthSystem>();
        scoreSystem = FindFirstObjectByType<ScoreSystem>();

        upgradePanel.SetActive(false);
    }

    private UpgradeData[] GetRandomUpgrades()
    {
        List<UpgradeData> pool = new List<UpgradeData>(upgradeDatabase.upgrades);
        UpgradeData[] selected = new UpgradeData[numberOfUpgrades];

        System.Random rand = new System.Random();

        for (int i = 0; i < numberOfUpgrades; i++)
        {
            if (pool.Count == 0) break;

            int randomIndex = rand.Next(pool.Count);
            selected[i] = pool[randomIndex];
            Debug.Log("Selected Upgrade: " + selected[i].header);

            pool.RemoveAt(randomIndex);
        }

        return selected;
    }

    public void ShowUpgrades()
    {
        if (upgradePanel == null || upgradePrefab == null) return;

        // Generate fresh upgrades every time panel is shown
        selectedUpgrades = GetRandomUpgrades();

        upgradePanel.SetActive(true);
        inGameSystem.isPaused = true;

        // Clear old children
        foreach (Transform child in upgradePanel.transform)
        {
            Destroy(child.gameObject);
        }

        int index = 0;
        foreach (UpgradeData upgrade in selectedUpgrades)
        {
            if (upgrade == null) continue;

            GameObject upgradeOption = Instantiate(upgradePrefab, upgradePanel.transform);

            // Set UI values
            ShowUpgrade showUpgrade = upgradeOption.GetComponent<ShowUpgrade>();
            if (showUpgrade != null)
            {
                showUpgrade.SetUpgrade(upgrade);
            }

            // Wire up button
            Button upgradeButton = upgradeOption.GetComponent<Button>();
            if (upgradeButton == null)
            {
                upgradeButton = upgradeOption.GetComponentInChildren<Button>();
            }

            if (upgradeButton != null)
            {
                UpgradeType type = upgrade.type;
                upgradeButton.onClick.RemoveAllListeners();
                upgradeButton.onClick.AddListener(() => HandleButtonClick(type));
            }

            // Position them in a row
            RectTransform rectTransform = upgradeOption.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                float spacing = 300f;
                float startX = -(selectedUpgrades.Length - 1) * spacing / 2f;
                rectTransform.anchoredPosition = new Vector2(startX + index * spacing, 0f);
            }

            index++;
        }
    }

    private void HandleButtonClick(UpgradeType type)
    {
        Debug.Log("Upgrade Selected: " + type);
        ApplyUpgrade(type);

        upgradePanel.SetActive(false);
        inGameSystem.isPaused = false;
        pauseSystem.ResumeGame();
        PlayerPrefs.SetInt("UpgradesSelected", PlayerPrefs.GetInt("UpgradesSelected", 0) + 1);
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Speed:
                carControl.maxSpeed *= 1.1f;
                break;

            case UpgradeType.Acceleration:
                carControl.acceleration *= 1.15f;
                break;

            case UpgradeType.Steering:
                carControl.steeringSpeed *= 1.1f;
                break;

            case UpgradeType.Health:
                if (healthSystem != null)
                {
                    healthSystem.maxHealth += 20;
                    healthSystem.Heal(20);
                    inGameSystem.UpdateHealthUI(healthSystem.currentHealth, healthSystem.maxHealth);
                }
                break;

            case UpgradeType.Repair:
                if (healthSystem != null)
                {
                    healthSystem.Heal(50);
                    inGameSystem.UpdateHealthUI(healthSystem.currentHealth, healthSystem.maxHealth);
                }
                break;

            case UpgradeType.XPValue:
                levelSystem.xpPerOrb *= 1.5f;
                break;

            case UpgradeType.MaxXPSpawn:
                levelSystem.maxOrbs = Mathf.Round(levelSystem.maxOrbs * 1.2f);
                xpSpawner.maxOrbs = levelSystem.maxOrbs;
                break;

            case UpgradeType.ScoreMultiplier:
                scoreSystem.scoreMultiplier += 1;
                break;

            default:
                Debug.Log("Upgrade Applied: " + type);
                break;
        }
    }
}
