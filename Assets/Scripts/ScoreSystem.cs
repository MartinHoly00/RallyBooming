using UnityEngine;
using System.Collections;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance;
    public TextMeshProUGUI scoreText;
    public int scoreMultiplier = 1;

    private int currentScore;
    private bool isScoring;
    private Coroutine scoreRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartScore();
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
            currentScore += 1 * scoreMultiplier;
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
}
