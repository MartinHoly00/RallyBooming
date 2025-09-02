using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float speed = 10f;
    public float turnSpeed = 50f;
    public Rigidbody rb;
    public Transform carTransform;
    public Camera carCamera;
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Handle input in Update for better responsiveness
        FixedUpdate();
        HandleInputs();

    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical") * speed;
        float turn = Input.GetAxis("Horizontal") * turnSpeed;

        Vector3 movement = carTransform.forward * move * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        Quaternion turnRotation = Quaternion.Euler(0f, turn * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Update wheel colliders
        frontLeftWheel.motorTorque = move;
        frontRightWheel.motorTorque = move;
        frontLeftWheel.steerAngle = turn;
        frontRightWheel.steerAngle = turn;

        // Update camera position
        carCamera.transform.position = carTransform.position + new Vector3(0, 5, -10);
        carCamera.transform.LookAt(carTransform);
    }

    void HandleInputs()
    {
        switch (Input.inputString)
        {
            case "w":
                rb.AddForce(carTransform.forward * speed);
                break;
            case "s":
                rb.AddForce(-carTransform.forward * speed);
                break;
            case "a":
                rb.AddTorque(-carTransform.up * turnSpeed);
                break;
            case "d":
                rb.AddTorque(carTransform.up * turnSpeed);
                break;
        }
    }

}
