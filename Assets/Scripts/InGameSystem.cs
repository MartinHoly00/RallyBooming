using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InGameSystem : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI healthText;
    public Slider healthBar;
    public Slider xpBar;
    public GameObject InGameUi;
    private HealthSystem healthSystem;
    private LevelSystem levelSystem;
    public bool isPaused = false;
    private GameManager gameManager;
    private PauseSystem pauseSystem;
    public bool isGameOver = false;

    private void Awake()
    {
        healthSystem = FindFirstObjectByType<HealthSystem>();
        levelSystem = FindFirstObjectByType<LevelSystem>();
        pauseSystem = FindFirstObjectByType<PauseSystem>();
    }

    private void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (healthSystem != null)
        {
            UpdateHealthUI(healthSystem.maxHealth, healthSystem.maxHealth);
        }
        if (levelSystem != null)
        {
            UpdateXPUI(levelSystem.currentXP, levelSystem.xpTrashold, levelSystem.currentLevel);
        }
        InGameUi.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGameOver) return;
            ToggleInGameUI(!InGameUi.activeInHierarchy);
            if (healthSystem.isDestroyed)
            {
                gameManager.resumeButton.SetActive(false);
                gameManager.SetHeaderText("Game Over");
            }
            else
            {
                gameManager.resumeButton.SetActive(true);
                gameManager.SetHeaderText("Paused");
            }
        }
    }

    public void UpdateHealthUI(float current, float max)
    {

        healthBar.maxValue = max;
        healthBar.value = current;

        healthText.text = $"Health: {current}";
    }

    public void UpdateXPUI(float current, float threshold, int level)
    {
        if (xpBar != null)
        {
            xpBar.maxValue = threshold;
            xpBar.value = current;
        }
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }
    }

    public void ToggleInGameUI(bool isActive)
    {
        if (isActive)
        {
            InGameUi.SetActive(true);
            gameManager.HideGameOverScreen();
            gameManager.resumeButton.SetActive(false);
            isPaused = false;
            pauseSystem.ResumeGame();
        }
        else
        {
            InGameUi.SetActive(false);
            gameManager.ShowGameOverScreen();
            gameManager.resumeButton.SetActive(true);
            isPaused = true;
            pauseSystem.PauseGame();
        }
    }

}
