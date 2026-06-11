using UnityEngine;

/// <summary>
/// Central place for one-shot sound effects (level up, pickups, environment hits).
/// Assign the clips in the Inspector on a single AudioManager object in the scene.
/// Call the helper methods from anywhere via AudioManager.Instance.
/// The looping engine sound is handled separately by <see cref="CarEngineSound"/>.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Output")]
    [Tooltip("AudioSource used to play 2D one-shot effects. If left empty one is created automatically.")]
    public AudioSource sfxSource;
    [Range(0f, 1f)]
    public float masterSfxVolume = 1f;

    [Header("Gameplay Clips")]
    public AudioClip levelUpClip;
    public AudioClip xpPickupClip;
    public AudioClip healthPickupClip;
    [Tooltip("Played when the car crashes into the environment. Volume scales with impact speed.")]
    public AudioClip environmentHitClip;

    [Header("Hit Settings")]
    [Tooltip("Impact speed (m/s) at which the hit sound reaches full volume.")]
    public float hitFullVolumeSpeed = 25f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f; // 2D
        }
    }

    /// <summary>Play a 2D one-shot clip at the given relative volume.</summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * masterSfxVolume);
    }

    public void PlayLevelUp() => PlaySFX(levelUpClip);

    public void PlayXPPickup() => PlaySFX(xpPickupClip);

    public void PlayHealthPickup() => PlaySFX(healthPickupClip);

    /// <summary>Environment crash sound, louder the faster the impact.</summary>
    public void PlayEnvironmentHit(float impactSpeed)
    {
        float volume = hitFullVolumeSpeed > 0.01f
            ? Mathf.Clamp01(impactSpeed / hitFullVolumeSpeed)
            : 1f;
        PlaySFX(environmentHitClip, volume);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
