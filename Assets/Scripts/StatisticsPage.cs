using TMPro;
using UnityEngine;

public class StatisticsPage : MonoBehaviour
{
    public int totalGamesPlayed;
    public int damageTaken;
    public int xpCollected;
    public int upgradesSelected;
    public int maxScore;

    public TextMeshProUGUI totalGamesPlayedText;
    public TextMeshProUGUI damageTakenText;
    public TextMeshProUGUI xpCollectedText;
    public TextMeshProUGUI upgradesSelectedText;
    public TextMeshProUGUI maxScoreText;

    void Start()
    {
        totalGamesPlayed = PlayerPrefs.GetInt("TotalGamesPlayed", 0);
        damageTaken = PlayerPrefs.GetInt("DamageTaken", 0);
        xpCollected = PlayerPrefs.GetInt("XPCollected", 0);
        upgradesSelected = PlayerPrefs.GetInt("UpgradesSelected", 0);
        maxScore = PlayerPrefs.GetInt("BestScore", 0);

        totalGamesPlayedText.text = "Total Games Played: " + totalGamesPlayed.ToString();
        damageTakenText.text = "Damage Taken: " + damageTaken.ToString();
        xpCollectedText.text = "XP Collected: " + xpCollected.ToString();
        upgradesSelectedText.text = "Upgrades Selected: " + upgradesSelected.ToString();
        maxScoreText.text = "Best score: " + maxScore.ToString();
    }
}
