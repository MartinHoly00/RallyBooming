using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneySystem : MonoBehaviour
{
    public int currentMoney;
    private ScoreSystem scoreSystem;

    public TextMeshProUGUI moneyText;

    void Start()
    {
        currentMoney = PlayerPrefs.GetInt("Money", 0);
        scoreSystem = FindFirstObjectByType<ScoreSystem>();
        moneyText.text = currentMoney.ToString() + "$";
    }

    public void AddMoney(int amout)
    {
        currentMoney += amout;
        moneyText.text = currentMoney.ToString() + "$";
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + amout);
    }
    public void SpendMoney(int amout)
    {
        if (currentMoney >= amout)
        {
            currentMoney -= amout;
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) - amout);
            Debug.Log("You spent money.");
        }
        Debug.Log("Not enough money.");
    }
}
