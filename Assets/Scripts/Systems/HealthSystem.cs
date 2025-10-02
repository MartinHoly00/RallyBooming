using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject destructionEffect;
    public GameObject carBody;
    public GameObject[] carWheels;
    public bool isDestroyed = false;
    public GameManager gameManager;
    private InGameSystem inGameSystem;

    private void Awake()
    {
        currentHealth = maxHealth;
    }
    private void Start()
    {
        inGameSystem = FindFirstObjectByType<InGameSystem>();
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("Current Health: " + currentHealth);
        PlayerPrefs.SetInt("DamageTaken", PlayerPrefs.GetInt("DamageTaken", 0) + (int)amount);

        if (inGameSystem != null)
            inGameSystem.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDestroyed) return;
        if (currentHealth + amount > maxHealth)
        {
            currentHealth = maxHealth;
        }

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log("Current Health: " + currentHealth);

        if (inGameSystem != null)
            inGameSystem.UpdateHealthUI(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
        if (carBody && carWheels != null)
        {
            foreach (var wheel in carWheels)
            {
                Destroy(wheel);
            }
            Destroy(carBody);
        }
        isDestroyed = true;
        inGameSystem.isGameOver = true;

        gameManager.ShowGameOverScreen();
        ScoreSystem.Instance.EndScore();
    }
}
