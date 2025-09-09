using UnityEngine;
using UnityEngine.UI;

public class GenerateUpgrades : MonoBehaviour
{
    //all upgrades
    public int numberOfUpgrades = 3;
    public Upgrade[] allUpgrades = new Upgrade[]{
        new Upgrade("Speed Boost", null, "Increases your top speed by 10%.", UpgradeType.Speed),
        new Upgrade("Acceleration Boost", null, "Increases your acceleration by 15%.", UpgradeType.Acceleration),
        new Upgrade("Health Increase", null, "Increases your maximum health by 20 points.", UpgradeType.Health),
        new Upgrade("Repair Kit", null, "Repairs 30 health points.", UpgradeType.Repair),
        new Upgrade("Steering Improvement", null, "Improves steering responsiveness by 10%.", UpgradeType.Steering),
        new Upgrade("Brake Enhancement", null, "Enhances braking power by 15%.", UpgradeType.Brake),
        new Upgrade("Nitro Boost", null, "Grants a temporary nitro boost for 5 seconds.", UpgradeType.Nitro),
        new Upgrade("XP Boost", null, "Increases XP gained from orbs by 20% for the next level.", UpgradeType.XPBoost),
    };
    public Upgrade[] selectedUpgrades;
    public GameObject upgradePanel;
    public GameObject upgradePrefab;
    public Button selectButton;

    private InGameSystem inGameSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inGameSystem = FindFirstObjectByType<InGameSystem>();
        selectedUpgrades = GetRandomUpgrades();
        upgradePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //

    public Upgrade[] GetRandomUpgrades()
    {
        Upgrade[] selectedUpgrades = new Upgrade[numberOfUpgrades];
        System.Random rand = new System.Random();
        int upgradesLength = allUpgrades.Length;

        for (int i = 0; i < numberOfUpgrades; i++)
        {
            int randomIndex = rand.Next(upgradesLength);
            selectedUpgrades[i] = allUpgrades[randomIndex];
            Debug.Log("Selected Upgrade: " + selectedUpgrades[i].header);
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
    }

    private void ApplyUpgrade(UpgradeType type)
    {
        // Add your upgrade application logic here
        switch (type)
        {
            case UpgradeType.Speed:
                // Apply speed boost
                Debug.Log("Speed Boost Applied!");
                break;
            case UpgradeType.Acceleration:
                // Apply acceleration boost
                Debug.Log("Acceleration Boost Applied!");
                break;
            case UpgradeType.Health:
                // Apply health increase
                Debug.Log("Health Increase Applied!");
                break;
            // Add other cases as needed
            default:
                Debug.Log("Upgrade Applied: " + type.ToString());
                break;
        }
    }
}
