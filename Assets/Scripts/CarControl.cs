using System;
using UnityEngine;

public class CarControl : MonoBehaviour
{
    public float acceleration = 20f;
    public float maxSpeed = 20f;
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

    private float turnSpeed;

    [Header("Camera Settings")]
    public float cameraDistance = 10f;
    public float cameraHeight = 5f;
    public float cameraFollowSpeed = 5f;
    public float cameraLookHeight = 1.5f;
    public float cameraLagOnTurn = 2f; // How much the camera lags during turns
    public float cameraOffsetOnTurn = 3f; // How far the camera moves to the outside of turns

    private Vector3 cameraVelocity;
    private float currentCameraLag = 0f;

    [Header("Braking")]
    public float brakeForce = 50f;   // Strength of braking
    public float brakeDrag = 5f;     // Extra drag when braking
    private float normalDrag;        // Default drag

    public HealthSystem healthSystem;

    private InGameSystem inGameSystem;

    private float deltaTime;

    void Awake()
    {
        deltaTime = Time.fixedDeltaTime * 1000f;
    }

    void Start()
    {
        rb.mass = 1000f;
        rb.linearDamping = 0.3f;
        rb.angularDamping = 3f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        normalDrag = rb.linearDamping; // Save default drag

        inGameSystem = FindFirstObjectByType<InGameSystem>();
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime * 60f; // Update deltaTime for FixedUpdate

        if (!healthSystem.isDestroyed && !inGameSystem.isPaused)
        {
            CameraFollow();

            turnSpeed = currentSpeed > 10f ? 50f : 70f;
            bool isGrounded;

            if (transform.eulerAngles.z > 45 && transform.eulerAngles.z < 315 || transform.eulerAngles.x > 45 && transform.eulerAngles.x < 315)
            {
                isGrounded = false;
            }
            else
            {
                isGrounded = true;
            }

            UpdateWheelVisuals();

            if (isGrounded)
            {
                PhysicsMovement();
                HandleCarMovement();
                HandleBraking();
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            carCamera.transform.position = carTransform.position - carTransform.forward * cameraDistance + Vector3
                .up * cameraHeight;
            carCamera.transform.LookAt(carTransform.position + Vector3.up * cameraLookHeight);

        }
    }

    void PhysicsMovement()
    {
        float move = Input.GetAxis("Vertical") * maxSpeed;
        float turn = Input.GetAxis("Horizontal");

        // Use AddForce instead of MovePosition for better collision handling
        Vector3 forceDirection = carTransform.forward * move * acceleration * deltaTime;
        rb.AddForce(forceDirection);

        // Limit max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        currentSpeed = rb.linearVelocity.magnitude;

        if (currentSpeed > 0.1f)
        {
            float dotProduct = Vector3.Dot(rb.linearVelocity.normalized, carTransform.forward);
            isMovingForward = dotProduct > 0;
        }

        // Use AddTorque instead of MoveRotation for steering
        if (currentSpeed > 0.5f) // Only turn when moving
        {
            float torqueAmount = turn * turnSpeed * currentSpeed * deltaTime;
            rb.AddTorque(carTransform.up * torqueAmount * currentSpeed / 2f);
        }

        // Calculate target steering angle for visual wheels
        float maxSteerAngle = 30f;
        float targetSteerAngle = turn * maxSteerAngle;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, steeringSpeed * deltaTime);

        UpdateWheelColliders(move);
    }

    void HandleBraking()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Apply opposite force to slow down
            Vector3 brakeForceVector = -rb.linearVelocity.normalized * brakeForce;
            rb.AddForce(brakeForceVector);

            // Increase drag while braking
            rb.linearDamping = brakeDrag;
        }
        else
        {
            // Reset drag when not braking
            rb.linearDamping = normalDrag;
        }
    }

    void UpdateWheelVisuals()
    {
        float steerAngle = currentSteerAngle;
        float rotationAngle = currentSpeed * Time.deltaTime * 360f / (2f * Mathf.PI * 0.3f);

        if (!isMovingForward) rotationAngle = -rotationAngle;
        wheelRollRotation += rotationAngle;

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

    void HandleCarMovement()
    {
        switch (Input.inputString)
        {
            case "w":
                rb.AddForce(carTransform.forward * maxSpeed);
                break;
            case "s":
                rb.AddForce(-carTransform.forward * maxSpeed);
                break;
            case "a":
                if (currentSpeed > 0) rb.AddTorque(-carTransform.up * turnSpeed);
                else rb.AddTorque(carTransform.up * turnSpeed);
                break;
            case "d":
                if (currentSpeed > 0) rb.AddTorque(carTransform.up * turnSpeed);
                else rb.AddTorque(-carTransform.up * turnSpeed);
                break;
        }
    }

    void CameraFollow()
    {
        float turnInput = Input.GetAxis("Horizontal");
        float targetLag = Mathf.Abs(turnInput) * cameraLagOnTurn;
        currentCameraLag = Mathf.Lerp(currentCameraLag, targetLag, Time.deltaTime * 3f);

        Vector3 turnOffset = carTransform.right * turnInput * cameraOffsetOnTurn;

        Vector3 basePosition = carTransform.position - carTransform.forward * cameraDistance + Vector3.up * cameraHeight;
        Vector3 desiredPosition = basePosition + turnOffset;

        float dynamicFollowSpeed = cameraFollowSpeed * (1f - currentCameraLag * 0.3f);

        carCamera.transform.position = Vector3.SmoothDamp(
            carCamera.transform.position,
            desiredPosition,
            ref cameraVelocity,
            1f / dynamicFollowSpeed
        );

        carCamera.transform.LookAt(carTransform.position + Vector3.up * cameraLookHeight);
    }

    void UpdateWheelColliders(float move)
    {
        frontLeftWheel.motorTorque = move;
        frontRightWheel.motorTorque = move;

        frontLeftWheel.steerAngle = currentSteerAngle;
        frontRightWheel.steerAngle = currentSteerAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enviroment"))
        {

            float damage = Mathf.Round(currentSpeed);
            if (damage < 5f) return;
            healthSystem.TakeDamage(damage / 2);
        }
    }
}
