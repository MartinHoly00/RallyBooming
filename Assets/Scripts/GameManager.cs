using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    public GameObject resumeButton;
    public TextMeshProUGUI headText;
    private InGameSystem inGameSystem;
    private PauseSystem pauseSystem;
    public TextMeshProUGUI scoreText;

    void Start()
    {
        pauseSystem = FindFirstObjectByType<PauseSystem>();
        inGameSystem = FindFirstObjectByType<InGameSystem>();
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        if (resumeButton != null)
        {
            resumeButton.SetActive(false);
        }
    }

    public void ReturnToGame()
    {
        if (inGameSystem != null)
        {
            inGameSystem.ToggleInGameUI(true);
            pauseSystem.ResumeGame();
            if (resumeButton != null)
            {
                resumeButton.SetActive(false);
            }
        }
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            inGameSystem.InGameUi.SetActive(false);
            pauseSystem.PauseGame();
            SetScoreText();
        }
    }

    public void HideGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
            inGameSystem.InGameUi.SetActive(true);
            pauseSystem.ResumeGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pauseSystem.ResumeGame();
        PlayerPrefs.SetInt("TotalGamesPlayed", PlayerPrefs.GetInt("TotalGamesPlayed", 0) + 1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        pauseSystem.ResumeGame();
    }

    public void SetHeaderText(string text)
    {
        if (headText != null)
        {
            headText.text = text;
        }
    }

    public void SetScoreText()
    {
        if (scoreText != null && ScoreSystem.Instance != null && inGameSystem.isGameOver)
        {
            scoreText.gameObject.SetActive(true);
            scoreText.text = "Yor score: " + ScoreSystem.Instance.GetScore().ToString();
        }
        else
        {
            scoreText.gameObject.SetActive(false);
        }
    }
}
