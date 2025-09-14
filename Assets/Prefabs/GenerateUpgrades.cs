using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GenerateUpgrades : MonoBehaviour
{
    //all upgrades
    public int numberOfUpgrades = 3;

    public List<Sprite> IconsForClient;
    private Upgrade[] allUpgrades;

    public Upgrade[] selectedUpgrades;
    public GameObject upgradePanel;
    public GameObject upgradePrefab;
    public Button selectButton;
    public GameObject xpSpawnerObject;

    private InGameSystem inGameSystem;
    private PauseSystem pauseSystem;
    private CarControl carControl;
    private LevelSystem levelSystem;
    private OrbSpawner xpSpawner;
    private HealthSystem healthSystem;
    private ScoreSystem scoreSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // Initialize allUpgrades here using IconsForClient
        allUpgrades = new Upgrade[]
        {
            new Upgrade("Speed Boost", IconsForClient.Count > 0 ? IconsForClient[0] : null, "Increases your top speed by 10%.", UpgradeType.Speed),
            new Upgrade("Acceleration Boost", IconsForClient.Count > 1 ? IconsForClient[1] : null, "Increases your acceleration by 15%.", UpgradeType.Acceleration),
            new Upgrade("Health Increase", IconsForClient.Count > 2 ? IconsForClient[2] : null, "Increases your maximum health by 20 points.", UpgradeType.Health),
            new Upgrade("Repair Kit", IconsForClient.Count > 3 ? IconsForClient[3] : null, "Repairs 50 health points.", UpgradeType.Repair),
            new Upgrade("Steering Improvement", IconsForClient.Count > 4 ? IconsForClient[4] : null, "Improves steering responsiveness by 10%.", UpgradeType.Steering),
            new Upgrade("XP Value Boost", IconsForClient.Count > 5 ? IconsForClient[5] : null, "Increases XP gained from orbs by 50%.", UpgradeType.XPValue),
            new Upgrade("XP Spawn Boost", IconsForClient.Count > 6 ? IconsForClient[6] : null, "Increases spawn of xp orbs by 20%.", UpgradeType.MaxXPSpawn),
            new Upgrade("Score Increase", IconsForClient.Count > 7 ? IconsForClient[7] : null, "Score increases by +1 more.", UpgradeType.ScoreMultiplier)
            //TODO - new Upgrade("Shield", IconsForClient.Count > 7 ? IconsForClient[7] : null, "Give you invincibility from meteorites for 15 seconds.", UpgradeType.Shield)
        };
    }

    void Start()
    {
        inGameSystem = FindFirstObjectByType<InGameSystem>();
        pauseSystem = FindFirstObjectByType<PauseSystem>();
        carControl = FindAnyObjectByType<CarControl>();
        levelSystem = FindFirstObjectByType<LevelSystem>();
        xpSpawner = xpSpawnerObject.GetComponent<OrbSpawner>();
        healthSystem = FindFirstObjectByType<HealthSystem>();
        scoreSystem = FindFirstObjectByType<ScoreSystem>();

        selectedUpgrades = GetRandomUpgrades();
        upgradePanel.SetActive(false);
    }

    public Upgrade[] GetRandomUpgrades()
    {
        Upgrade[] selectedUpgrades = new Upgrade[numberOfUpgrades];
        System.Random rand = new System.Random();
        int upgradesLength = allUpgrades.Length;

        // Create a list of available indices
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < upgradesLength; i++)
        {
            availableIndices.Add(i);
        }

        for (int i = 0; i < numberOfUpgrades; i++)
        {
            if (availableIndices.Count == 0) break; // Safety check

            int randomListIndex = rand.Next(availableIndices.Count);
            int upgradeIndex = availableIndices[randomListIndex];
            selectedUpgrades[i] = allUpgrades[upgradeIndex];
            Debug.Log("Selected Upgrade: " + selectedUpgrades[i].header);

            // Remove the used index so it can't be picked again
            availableIndices.RemoveAt(randomListIndex);
        }

        return selectedUpgrades;
    }
    public void ShowUpgrades()
    {
        if (upgradePanel != null && upgradePrefab != null)
        {
            upgradePanel.SetActive(true);
            inGameSystem.isPaused = true;

            // Clear existing upgrade options
            foreach (Transform child in upgradePanel.transform)
            {
                Destroy(child.gameObject);
            }

            int index = 0;
            foreach (Upgrade upgrade in selectedUpgrades)
            {
                GameObject upgradeOption = Instantiate(upgradePrefab, upgradePanel.transform);
                ShowUpgrade showUpgrade = upgradeOption.GetComponent<ShowUpgrade>();
                if (showUpgrade != null)
                {
                    showUpgrade.SetUpgrade(upgrade);
                }

                // Get the button component from the upgrade prefab
                Button upgradeButton = upgradeOption.GetComponent<Button>();
                if (upgradeButton == null)
                {
                    upgradeButton = upgradeOption.GetComponentInChildren<Button>();
                }

                // Set up the button click event with the specific upgrade type
                if (upgradeButton != null)
                {
                    UpgradeType upgradeType = upgrade.type; // Capture the upgrade type
                    upgradeButton.onClick.RemoveAllListeners(); // Clear existing listeners
                    upgradeButton.onClick.AddListener(() => HandleButtonClick(upgradeType));
                }

                // Position upgrades in a row with even gaps
                RectTransform rectTransform = upgradeOption.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    float spacing = 300f; // Adjust spacing between upgrades
                    float startX = -(selectedUpgrades.Length - 1) * spacing / 2f; // Center the row

                    rectTransform.anchoredPosition = new Vector2(startX + index * spacing, 0f);
                }

                index++;
            }
        }
    }

    public void HandleButtonClick(UpgradeType type)
    {
        Debug.Log("Upgrade Selected: " + type.ToString());

        // Apply the upgrade effect here
        ApplyUpgrade(type);

        upgradePanel.SetActive(false);
        inGameSystem.isPaused = false;
        pauseSystem.ResumeGame();
        PlayerPrefs.SetInt("UpgradesSelected", PlayerPrefs.GetInt("UpgradesSelected", 0) + 1);
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        // Add your upgrade application logic here
        switch (type)
        {
            case UpgradeType.Speed:
                carControl.maxSpeed *= 1.1f;
                Debug.Log("Speed Boost Applied!");
                break;

            case UpgradeType.Acceleration:
                carControl.acceleration *= 1.15f;
                Debug.Log("Acceleration Boost Applied!");
                break;

            case UpgradeType.Steering:
                carControl.steeringSpeed = carControl.steeringSpeed * 1.1f;
                break;

            case UpgradeType.Health:
                if (healthSystem != null)
                {
                    healthSystem.maxHealth += 20;
                    healthSystem.Heal(20); // Optionally heal the player by the same amount
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
                Debug.Log("Upgrade Applied: " + type.ToString());
                break;
        }
    }

    List<int> GetIconsForClient()
    {
        List<int> icons = new List<int>();
        foreach (Upgrade upgrade in selectedUpgrades)
        {
            icons.Add((int)upgrade.type);
        }
        return icons;
    }
}
