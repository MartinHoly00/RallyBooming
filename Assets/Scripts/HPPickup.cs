using UnityEngine;

public class HPPickup : MonoBehaviour
{
    public GameObject pickupEffect;
    public float healthAmount = 20f;
    private OrbSpawner orbSpawner;
    private HealthSystem healthSystem;

    private void Start()
    {
        healthSystem = FindFirstObjectByType<HealthSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            healthSystem.Heal(healthAmount);
            if (pickupEffect != null)
            {
                GameObject explosion = Instantiate(pickupEffect, transform.position, transform.rotation);
                float effectDuration = explosion.GetComponent<ParticleSystem>().main.duration;
                Destroy(explosion, effectDuration);
            }
            Destroy(gameObject);
        }
    }
}
