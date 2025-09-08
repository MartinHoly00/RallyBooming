using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("Meteorite Settings")]
    public float damage = 100f;
    public GameObject explosionEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerParent"))
        {
            Debug.Log("Direct hit on Player-tagged object!");
            HandleCarHit(collision);
        }
        else
        {
            HandleGroundHit(collision);
        }
    }

    private void HandleCarHit(Collision collision, Transform playerTransform = null)
    {
        Rigidbody carRb = collision.rigidbody;

        if (carRb == null && playerTransform != null)
        {
            carRb = playerTransform.GetComponent<Rigidbody>();
        }

        if (carRb != null)
        {
            Transform carTransform = playerTransform ?? collision.transform;
            Vector3 impactForce = transform.position - carTransform.position;
            impactForce.y = 0;

            HealthSystem hs = collision.gameObject.GetComponent<HealthSystem>();

            if (!hs)
            {
                hs = carTransform.GetComponentInChildren<HealthSystem>();
            }

            if (hs != null)
            {
                hs.TakeDamage(damage);
            }

            carRb.AddForce(-impactForce.normalized * 10000f, ForceMode.Impulse);
        }

        SpawnExplosion();
        Destroy(gameObject);
    }

    private void HandleGroundHit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            SpawnExplosion();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enviroment"))
        {
            //launch meteorite upwards and sideways
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
                rb.linearVelocity = randomDirection * 20f;
            }
        }
    }

    private void SpawnExplosion()
    {
        if (explosionEffect == null) return;

        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Calculate longest particle system lifetime
        float maxLifetime = 0f;
        foreach (ParticleSystem ps in explosion.GetComponentsInChildren<ParticleSystem>())
        {
            float duration = ps.main.duration + ps.main.startLifetime.constantMax;
            if (duration > maxLifetime)
            {
                maxLifetime = duration;
            }
        }

        Destroy(explosion, maxLifetime);
    }
}
