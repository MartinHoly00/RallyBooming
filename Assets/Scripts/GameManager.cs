using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    private InGameSystem inGameSystem;
    public GameObject resumeButton;
    private PauseSystem pauseSystem;

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
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        pauseSystem.ResumeGame();
    }
}
