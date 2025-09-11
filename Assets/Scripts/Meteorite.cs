using UnityEngine;

public class Meteorite : MonoBehaviour
{
    [Header("Meteorite Settings")]
    public float damage = 100;
    public GameObject explosionEffect;
    public SphereCollider sphereCollider;

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
        HealthSystem hs = collision.gameObject.GetComponent<HealthSystem>();

        if (carRb == null && playerTransform != null)
        {
            carRb = playerTransform.GetComponent<Rigidbody>();
        }
        Destroy(sphereCollider);
        SpawnExplosion();
        Destroy(gameObject);

        if (carRb != null && hs != null)
        {

            hs.TakeDamage(damage); //TODO - when hit, car explodes even if it has health left
        }
    }

    private void HandleGroundHit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            SpawnExplosion();
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enviroment"))
        {
            Debug.Log("Destroying Enviroment-tagged object!");
            GameObject hitObject = collision.gameObject;
            SpawnExplosion();
            Destroy(hitObject);
            Destroy(gameObject);
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
