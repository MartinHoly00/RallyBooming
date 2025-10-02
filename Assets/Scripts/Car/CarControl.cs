using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("Config")]
    public CarData carData;

    public float acceleration = 20f;
    public float maxSpeed = 30f;
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
    private float carMass = 1000f;

    private float turnSpeed;

    [Header("Camera Settings")]
    public float cameraDistance = 10f;
    public float cameraHeight = 5f;
    public float cameraFollowSpeed = 5f;
    public float cameraLookHeight = 1.5f;
    public float cameraLagOnTurn = 2f;
    public float cameraOffsetOnTurn = 3f;

    private Vector3 cameraVelocity;
    private float currentCameraLag = 0f;

    [Header("Braking")]
    public float brakeForce = 0.1f;
    public float brakeDrag = 100f;
    private float normalDrag;

    public HealthSystem healthSystem;

    private float deltaTime;
    //private bool moveForward, moveBackward, turnLeft, turnRight;

    void Awake()
    {
        deltaTime = Time.fixedDeltaTime * 1000f;
    }

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb == null) rb = GetComponentInChildren<Rigidbody>();

        rb.mass = carMass;
        rb.linearDamping = 0.3f;
        rb.angularDamping = 3f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        normalDrag = rb.linearDamping;
    }

    void Update()
    {
        if (frontLeftWheel == null || frontRightWheel == null || rearLeftWheel == null || rearRightWheel == null)
        {
            GameObject carWheelFL = GameObject.FindWithTag("CarWheelFL");
            GameObject carWheelFR = GameObject.FindWithTag("CarWheelFR");
            GameObject carWheelRL = GameObject.FindWithTag("CarWheelRL");
            GameObject carWheelRR = GameObject.FindWithTag("CarWheelRR");

            if (carWheelFL != null) frontLeftWheel = carWheelFL.GetComponent<WheelCollider>();
            if (carWheelFR != null) frontRightWheel = carWheelFR.GetComponent<WheelCollider>();
            if (carWheelRL != null) rearLeftWheel = carWheelRL.GetComponent<WheelCollider>();
            if (carWheelRR != null) rearRightWheel = carWheelRR.GetComponent<WheelCollider>();

            frontLeftWheelMesh = carWheelFL.transform;
            frontRightWheelMesh = carWheelFR.transform;
            rearLeftWheelMesh = carWheelRL.transform;
            rearRightWheelMesh = carWheelRR.transform;
        }
    }

    private void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime * 60f; // Update deltaTime for FixedUpdate

        if (!healthSystem.isDestroyed)
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
        if (Input.GetKey(KeyCode.Space)) return;

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
        if (Input.GetKey(KeyCode.Space) && rb.linearVelocity.magnitude > 0.1f)
        {
            // Add a braking force opposite to velocity (scales with speed)
            Vector3 brakeForceVector = -rb.linearVelocity.normalized * brakeForce * currentSpeed;
            rb.AddForce(brakeForceVector, ForceMode.Acceleration);

            // Add *slightly increased drag* to simulate resistance
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, normalDrag + 2f, Time.fixedDeltaTime * 2f);
        }
        else
        {
            // Go back to normal drag when not braking
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, normalDrag, Time.fixedDeltaTime * 2f);
        }
    }


    void UpdateWheelVisuals()
    {
        float rotationAngle = currentSpeed * Time.deltaTime * 360f / (2f * Mathf.PI * 0.3f);
        if (!isMovingForward) rotationAngle = -rotationAngle;
        wheelRollRotation += rotationAngle;

        Quaternion rollRotation = Quaternion.Euler(wheelRollRotation, 0f, 0f);

        rearLeftWheelMesh.localRotation = rollRotation;
        rearRightWheelMesh.localRotation = rollRotation;

        Quaternion steerRotation = Quaternion.Euler(0f, currentSteerAngle, 0f);
        Quaternion combinedRotation = steerRotation * rollRotation;
        frontLeftWheelMesh.localRotation = combinedRotation;
        frontRightWheelMesh.localRotation = combinedRotation;
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
            case "space":
                // Handled in HandleBraking
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

    /*     public void PressForward() => moveForward = true;
        public void ReleaseForward() => moveForward = false;
        public void PressBackward() => moveBackward = true;
        public void ReleaseBackward() => moveBackward = false;
        public void PressLeft() => turnLeft = true;
        public void ReleaseLeft() => turnLeft = false;
        public void PressRight() => turnRight = true;
        public void ReleaseRight() => turnRight = false; */

    public void ApplyCarData(CarData data)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (rb != null)
            rb.mass = data.mass;

        acceleration = data.acceleration;
        maxSpeed = data.maxSpeed;
        steeringSpeed = data.steeringSpeed;
        brakeForce = data.brakeForce;
        brakeDrag = data.brakeDrag;
    }
}
