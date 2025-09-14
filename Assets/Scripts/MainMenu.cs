using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject statisticsUI;

    void Start()
    {
        mainMenuUI.SetActive(true);
        statisticsUI.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("TotalGamesPlayed", PlayerPrefs.GetInt("TotalGamesPlayed", 0) + 1);
    }

    public void OpenStatistics()
    {
        mainMenuUI.SetActive(false);
        statisticsUI.SetActive(true);
    }


    public void CloseStatistics()
    {
        statisticsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
