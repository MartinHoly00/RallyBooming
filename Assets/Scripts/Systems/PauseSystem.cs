using UnityEngine;

public class PauseSystem : MonoBehaviour
{
    public bool isPaused = false;

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        ScoreSystem.Instance.PauseScore();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        ScoreSystem.Instance.UnpauseScore();
    }

}
