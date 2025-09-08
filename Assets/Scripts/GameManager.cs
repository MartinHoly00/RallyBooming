using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverScreen;
    private InGameSystem inGameSystem;
    public GameObject resumeButton;

    void Start()
    {
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
        }
    }

    public void HideGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
