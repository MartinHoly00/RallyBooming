using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject statisticsUI;
    public GameObject shopUI;

    void Start()
    {
        mainMenuUI.SetActive(true);
        statisticsUI.SetActive(false);
        shopUI.SetActive(false);
        //TODO - 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("TotalGamesPlayed", PlayerPrefs.GetInt("TotalGamesPlayed", 0) + 1);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenStatistics()
    {
        mainMenuUI.SetActive(false);
        statisticsUI.SetActive(true);
    }

    public void OpenShop()
    {
        mainMenuUI.SetActive(false);
        shopUI.SetActive(true);
    }


    public void CloseStatistics()
    {
        statisticsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
