using UnityEngine;

/// <summary>
/// Drives a looping engine AudioSource so its pitch and volume rise with the car's speed.
/// Put this on the car (next to CarControl) and assign a looping engine clip to the AudioSource.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class CarEngineSound : MonoBehaviour
{
    [Tooltip("Car whose speed drives the engine sound. Auto-found on the same object/children if empty.")]
    public CarControl car;
    public AudioSource engineSource;

    [Header("Pitch")]
    [Tooltip("Engine pitch while idle / stopped.")]
    public float idlePitch = 0.8f;
    [Tooltip("Engine pitch at top speed.")]
    public float maxPitch = 2.2f;

    [Header("Volume")]
    [Tooltip("Engine volume while idle / stopped.")]
    [Range(0f, 1f)] public float idleVolume = 0.35f;
    [Tooltip("Engine volume at top speed.")]
    [Range(0f, 1f)] public float maxVolume = 1f;

    [Tooltip("How quickly pitch/volume follow speed changes (higher = snappier).")]
    public float responsiveness = 6f;

    private void Start()
    {
        if (engineSource == null) engineSource = GetComponent<AudioSource>();
        if (car == null) car = GetComponentInParent<CarControl>();
        if (car == null) car = GetComponentInChildren<CarControl>();

        engineSource.loop = true;
        engineSource.playOnAwake = false;
        if (engineSource.clip != null && !engineSource.isPlaying)
            engineSource.Play();
    }

    private void Update()
    {
        if (engineSource == null || car == null) return;

        // 0 at standstill, 1 at top speed.
        float speedRatio = car.maxSpeed > 0.01f
            ? Mathf.Clamp01(car.currentSpeed / car.maxSpeed)
            : 0f;

        float targetPitch = Mathf.Lerp(idlePitch, maxPitch, speedRatio);
        float targetVolume = Mathf.Lerp(idleVolume, maxVolume, speedRatio);

        float t = responsiveness * Time.deltaTime;
        engineSource.pitch = Mathf.Lerp(engineSource.pitch, targetPitch, t);
        engineSource.volume = Mathf.Lerp(engineSource.volume, targetVolume, t);
    }
}
