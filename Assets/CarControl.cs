using System;
using Unity.VisualScripting;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float speed = 100f;
    public float acceleration = 10f;
    public float maxSpeed = 200f;
    public float turnSpeed = 50f;
    public float steeringSpeed = 5f;
    public Rigidbody rb;
    public Transform carTransform;
    public Camera carCamera;
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftWheel;
    public WheelCollider frontRightWheel;
    public WheelCollider rearLeftWheel;
    public WheelCollider rearRightWheel;
    [Header("Wheel Meshes")]
    public Transform frontLeftWheelMesh;
    public Transform frontRightWheelMesh;
    public Transform rearLeftWheelMesh;
    public Transform rearRightWheelMesh;

    public float currentSpeed = 0f;
    public bool isMovingForward = true;
    private float wheelRollRotation = 0f;
    private float currentSteerAngle = 0f;
    [Header("Car Settings")]
    public float motorForce = 1500f;
    public float brakeForce = 3000f;

    public Transform centerOfMass;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FixedUpdate();
        HandleInputs();
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Vertical") * speed;
        float turn = Input.GetAxis("Horizontal");

        Vector3 movement = carTransform.forward * move * Time.deltaTime;
        rb.MovePosition(rb.position + movement);

        currentSpeed = movement.magnitude / Time.deltaTime;

        if (currentSpeed > 0.1f)
        {
            float dotProduct = Vector3.Dot(movement.normalized, carTransform.forward);
            isMovingForward = dotProduct > 0;
        }

        if (currentSpeed > 0)
        {
            Quaternion turnRotation = Quaternion.Euler(0f, turn * turnSpeed * Time.deltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Calculate target steering angle
        float maxSteerAngle = 30f;
        float targetSteerAngle = turn * maxSteerAngle;

        // Smoothly interpolate to target steering angle
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, steeringSpeed * Time.deltaTime);

        // Update wheel colliders
        frontLeftWheel.motorTorque = move;
        frontRightWheel.motorTorque = move;

        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;

        UpdateWheelVisuals();

        // Update camera position
        carCamera.transform.position = carTransform.position + new Vector3(0, 5, -10);
        carCamera.transform.LookAt(carTransform);
    }

    void UpdateWheelVisuals()
    {
        // Use the smooth current steering angle instead of wheel collider angle
        float steerAngle = currentSteerAngle;

        // Calculate wheel roll rotation
        float rotationAngle = currentSpeed * Time.deltaTime * 360f / (2f * Mathf.PI * 0.3f);

        if (!isMovingForward)
        {
            rotationAngle = -rotationAngle;
        }

        wheelRollRotation += rotationAngle;

        // Apply combined rotation to all wheels using Quaternion multiplication
        Quaternion rollRotation = Quaternion.Euler(wheelRollRotation, 0f, 0f);

        // Rear wheels (only roll)
        rearLeftWheelMesh.transform.localRotation = rollRotation;
        rearRightWheelMesh.transform.localRotation = rollRotation;

        // Front wheels (roll + steer)
        Quaternion steerRotation = Quaternion.Euler(0f, steerAngle, 0f);
        Quaternion combinedRotation = steerRotation * rollRotation;

        frontLeftWheelMesh.transform.localRotation = combinedRotation;
        frontRightWheelMesh.transform.localRotation = combinedRotation;
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
            //nefunguje
            case "a":
                if (currentSpeed > 0)
                {
                    rb.AddTorque(-carTransform.up * turnSpeed);
                }
                else
                {
                    rb.AddTorque(carTransform.up * turnSpeed);
                }
                break;
            case "d":
                if (currentSpeed > 0)
                {
                    rb.AddTorque(carTransform.up * turnSpeed);
                }
                else
                {
                    rb.AddTorque(-carTransform.up * turnSpeed);
                }
                break;
        }
    }

}
