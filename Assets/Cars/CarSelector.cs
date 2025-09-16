using UnityEngine;

public class CarSelector : MonoBehaviour
{
    [Tooltip("Database containing all CarData ScriptableObjects.")]
    public CarDatabase carDatabase;

    [Tooltip("Reference to your CarControl component.")]
    public CarControl carControl;

    [Tooltip("PlayerPrefs key that stores chosen car id.")]
    public string playerPrefsKey = "SelectedCarId";

    [Tooltip("If PlayerPrefs key is missing or invalid, fallback to this id (optional).")]
    public string fallbackCarId = "";

    [Tooltip("Optional: transform where the bodyPrefab will be instantiated (visual only).")]
    public Transform bodyMount;

    void Start()
    {
        if (carDatabase == null)
        {
            Debug.LogError("CarSelector: CarDatabase is not set.");
            return;
        }

        if (carControl == null)
        {
            Debug.LogError("CarSelector: CarControl reference is not set.");
            return;
        }

        string selectedId = PlayerPrefs.GetString(playerPrefsKey, fallbackCarId);
        CarData data = carDatabase.GetById(selectedId);

        if (data == null)
        {
            if (carDatabase.cars.Count > 0)
            {
                Debug.LogWarning($"CarSelector: Car id '{selectedId}' not found. Falling back to first car in database.");
                data = carDatabase.cars[0];
            }
            else
            {
                Debug.LogError("CarSelector: CarDatabase has no entries.");
                return;
            }
        }

        // Apply scriptable object values to the CarControl
        carControl.ApplyCarData(data);

        // Optionally instantiate visual body prefab (remove physics components to keep visuals safe)
        if (bodyMount != null && data.bodyPrefab != null)
        {
            GameObject inst = Instantiate(data.bodyPrefab, bodyMount);
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localRotation = Quaternion.identity;
            inst.transform.localScale = Vector3.one;

            // remove physics from the visual prefab so it doesn't interfere with the Rigidbody on the car root
            foreach (var rb in inst.GetComponentsInChildren<Rigidbody>()) Destroy(rb);
        }
    }
}
