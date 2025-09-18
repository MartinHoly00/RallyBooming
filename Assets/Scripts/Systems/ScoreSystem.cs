using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;
    public TextMeshProUGUI scoreText;
    public int scoreMultiplier = 1;

    private int currentScore;
    private bool isScoring;
    private Coroutine scoreRoutine;
    private CarControl carControl;
    private int speedMultiplaier = 1;
    public TextMeshProUGUI speedMultiText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        carControl = FindFirstObjectByType<CarControl>();
        StartScore();
        speedMultiText.gameObject.SetActive(true);
    }

    void Update()
    {
        CalculateSpeedMiltiplaier();
    }

    public void StartScore()
    {
        if (isScoring) return;

        currentScore = 0;
        scoreText.text = "Score:" + currentScore.ToString();
        isScoring = true;
        scoreRoutine = StartCoroutine(ScorePerSecond());
        Debug.Log("Score started.");
    }

    public void PauseScore()
    {
        if (!isScoring) return;

        isScoring = false;
        if (scoreRoutine != null)
            StopCoroutine(scoreRoutine);

        Debug.Log("Score paused at: " + currentScore);
    }

    public void UnpauseScore()
    {
        if (isScoring) return;
        isScoring = true;
        scoreRoutine = StartCoroutine(ScorePerSecond());
        Debug.Log("Score resumed at: " + currentScore);
    }

    private IEnumerator ScorePerSecond()
    {
        while (isScoring)
        {
            currentScore += 1 * scoreMultiplier * speedMultiplaier;
            scoreText.text = "Score: " + currentScore.ToString();
            yield return new WaitForSeconds(1f);
        }
    }

    public void EndScore()
    {
        isScoring = false;
        if (scoreRoutine != null)
            StopCoroutine(scoreRoutine);

        Debug.Log("Final Score: " + currentScore);

        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
            Debug.Log("New High Score!");
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    private void CalculateSpeedMiltiplaier()
    {
        float speed = carControl.currentSpeed;
        if (speed > 20 && speed < 30)
        {
            speedMultiplaier = 2;
            speedMultiText.gameObject.SetActive(true);
            speedMultiText.color = Color.yellow;
            speedMultiText.text = speedMultiplaier.ToString() + "x";
        }
        else if (speed > 30 && speed < 50)
        {
            speedMultiplaier = 3;
            speedMultiText.gameObject.SetActive(true);
            speedMultiText.color = new Color(1f, 0.5f, 0f);
            speedMultiText.text = speedMultiplaier.ToString() + "x";
        }
        else if (speed > 50)
        {
            speedMultiplaier = 4;
            speedMultiText.gameObject.SetActive(true);
            speedMultiText.color = Color.red;
            speedMultiText.text = speedMultiplaier.ToString() + "x";
        }
        else
        {
            speedMultiplaier = 1;
            speedMultiText.gameObject.SetActive(false);
        }
    }
}
