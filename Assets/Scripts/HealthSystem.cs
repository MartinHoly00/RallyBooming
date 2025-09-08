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

    private void Start()
    {
        currentHealth = maxHealth;
        inGameSystem = FindFirstObjectByType<InGameSystem>();
    }

    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("Current Health: " + currentHealth);

        if (inGameSystem != null)
            inGameSystem.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDestroyed)
        {
            Die();
        }
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

        gameManager.ShowGameOverScreen();
    }
}
