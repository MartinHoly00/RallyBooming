/* using Unity.VisualScripting;
using UnityEngine;

public class WheelControls : MonoBehaviour
{
    private CarControl carControl;

    //later calculate based on car speed
    private float rotationSpeed = 0f;
    private float currentRotation = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the CarControl component from parent object
        carControl = GetComponentInParent<CarControl>();

        transform.Rotate(currentRotation, 0f, 0f, Space.Self);

        if (carControl == null)
        {
            Debug.LogError("CarControl component not found in parent objects!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (carControl.acceleration > 0)
            rotationSpeed = carControl.currentSpeed;
        else
            rotationSpeed = 0;
        RotateWheels();
    }

    void RotateWheels()
    {
        if (carControl != null)
        {
            float rotationAngle = rotationSpeed * Time.deltaTime * 360f / (2f * Mathf.PI * 0.3f); // Assuming wheel radius is 0.3 meters
            currentRotation += rotationAngle;

            transform.Rotate(rotationAngle, 0f, 0f, Space.Self);
        }

    }
}
 */