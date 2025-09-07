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

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Health: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
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
        //open game over screen
    }
}
