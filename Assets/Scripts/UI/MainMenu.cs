using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject statisticsUI;
    public GameObject shopUI;
    public GameObject mapsUI;

    [Header("Map Selection")]
    public MapDatabase mapDatabase; // Assign in inspector

    void Start()
    {
        mainMenuUI.SetActive(true);
        statisticsUI.SetActive(false);
        shopUI.SetActive(false);
        mapsUI.SetActive(false);
        //TODO - 
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGame()
    {
        string sceneName = GetSelectedSceneName();
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("MainMenu: No map selected, loading default SampleScene");
            sceneName = "SampleScene";
        }
        
        SceneManager.LoadScene(sceneName);
        PlayerPrefs.SetInt("TotalGamesPlayed", PlayerPrefs.GetInt("TotalGamesPlayed", 0) + 1);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private string GetSelectedSceneName()
    {
        if (mapDatabase == null) return null;
        return MapSelector.GetSelectedSceneNameFromPrefs(mapDatabase);
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

    public void OpenMaps()
    {
        mainMenuUI.SetActive(false);
        mapsUI.SetActive(true);
    }

    public void CloseMaps()
    {
        mapsUI.SetActive(false);
        mainMenuUI.SetActive(true);
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
