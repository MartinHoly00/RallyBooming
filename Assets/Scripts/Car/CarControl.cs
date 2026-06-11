using UnityEngine;

public class CarControl : MonoBehaviour
{
    [Header("Config")]
    public CarData carData;

    public float acceleration = 20f;
    public float maxSpeed = 30f;
    public float steeringSpeed = 110f; // turn rate in degrees/sec at speed (overwritten by CarData)

    [Header("Handling")]
    [Tooltip("How strongly tyres resist sideways sliding. Higher = more grip, lower = more drift.")]
    public float grip = 4f;
    [Tooltip("Speed (m/s) at which steering reaches full strength. Below this, steering scales down.")]
    public float steeringFullSpeedAt = 6f;
    [Tooltip("Reverse top speed as a fraction of maxSpeed.")]
    public float reverseSpeedFactor = 0.45f;
    [Range(0f, 1f)]
    [Tooltip("Fraction of top speed where tyres start losing grip and the car begins to drift.")]
    public float driftStartSpeedFactor = 0.6f;
    [Range(0f, 1f)]
    [Tooltip("Grip kept at top speed, as a fraction of normal grip. Lower = more slippery when fast.")]
    public float highSpeedGripFactor = 0.15f;
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

    [Header("Camera Settings")]
    public float cameraDistance = 10f;
    public float cameraHeight = 5f;
    public float cameraFollowSpeed = 5f;
    public float cameraLookHeight = 1.5f;
    public float cameraLagOnTurn = 2f;
    public float cameraOffsetOnTurn = 3f;

    public float cameraLookSmoothTime = 0.06f;

    private Vector3 cameraVelocity;
    private float currentCameraLag = 0f;
    private Vector3 cameraLookVelocity;
    private Vector3 smoothedLookTarget;

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

        // The car is moved by physics in FixedUpdate (50 Hz) but the camera follows
        // it per rendered frame in LateUpdate. Without interpolation the visual
        // transform only updates on physics steps, so the camera chases a "stepping"
        // target and stutters. Interpolation smooths the transform between steps.
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        normalDrag = rb.linearDamping;

        smoothedLookTarget = carTransform.position + Vector3.up * cameraLookHeight;
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
                HandleBraking();
            }
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void LateUpdate()
    {
        if (healthSystem == null) return;

        if (healthSystem.isDestroyed)
        {
            // Settle into a clean static-ish shot when the car is wrecked
            Vector3 flatForward = GetFlatForward();
            Vector3 desiredPosition = carTransform.position - flatForward * cameraDistance + Vector3.up * cameraHeight;
            carCamera.transform.position = Vector3.SmoothDamp(
                carCamera.transform.position, desiredPosition, ref cameraVelocity, 1f / cameraFollowSpeed);
            UpdateLookAt();
        }
        else
        {
            CameraFollow();
        }
    }

    void PhysicsMovement()
    {
        float throttle = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        bool braking = Input.GetKey(KeyCode.Space);

        // Work in the car's heading flattened onto the ground so ramps/bumps
        // don't steal drive force or throw off the grip direction.
        Vector3 flatForward = Vector3.ProjectOnPlane(carTransform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(carTransform.right, Vector3.up).normalized;

        Vector3 velocity = rb.linearVelocity;
        float forwardSpeed = Vector3.Dot(velocity, flatForward);
        float sidewaysSpeed = Vector3.Dot(velocity, flatRight);

        currentSpeed = velocity.magnitude;
        isMovingForward = forwardSpeed >= 0f;

        // --- Drive: push along the heading until we reach the speed cap ---
        if (!braking && Mathf.Abs(throttle) > 0.01f)
        {
            float speedCap = throttle >= 0f ? maxSpeed : maxSpeed * reverseSpeedFactor;
            if (Mathf.Abs(forwardSpeed) < speedCap)
            {
                // Same force magnitude as before, so each car keeps its acceleration feel.
                rb.AddForce(flatForward * throttle * maxSpeed * acceleration * deltaTime);
            }
        }

        // --- Lateral grip: bleed off sideways velocity so the car follows its
        // nose instead of sliding. This is what makes it feel planted, not icy.
        // Above driftStartSpeedFactor of top speed the tyres progressively lose
        // grip (fading toward highSpeedGripFactor), so the car gets loose and
        // drifts when pushed hard at speed - like the old slippery feel.
        float speedRatio = maxSpeed > 0.01f ? currentSpeed / maxSpeed : 0f;
        float driftBlend = Mathf.InverseLerp(driftStartSpeedFactor, 1f, speedRatio);
        float effectiveGrip = grip * Mathf.Lerp(1f, highSpeedGripFactor, driftBlend);
        float gripFactor = Mathf.Clamp01(effectiveGrip * Time.fixedDeltaTime);
        rb.AddForce(-flatRight * sidewaysSpeed * gripFactor, ForceMode.VelocityChange);

        // --- Steering: rotate the car. Effectiveness ramps with speed and then
        // holds steady (linear, not squared) so it's responsive without being
        // twitchy at top speed. Steering inverts when reversing.
        if (currentSpeed > 0.3f)
        {
            float steerFactor = Mathf.Clamp01(currentSpeed / steeringFullSpeedAt);
            float reverseSign = isMovingForward ? 1f : -1f;
            float yaw = turn * steeringSpeed * steerFactor * reverseSign * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(yaw, Vector3.up));
        }

        // Visual front-wheel steering angle.
        float targetSteerAngle = turn * 30f;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, 8f * Time.fixedDeltaTime);

        UpdateWheelColliders(throttle * maxSpeed);
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

    void CameraFollow()
    {
        float turnInput = Input.GetAxis("Horizontal");
        float targetLag = Mathf.Abs(turnInput) * cameraLagOnTurn;
        currentCameraLag = Mathf.Lerp(currentCameraLag, targetLag, Time.deltaTime * 3f);

        // Use a flattened (yaw-only) forward so bumps that pitch/roll the car
        // don't tilt or shake the camera.
        Vector3 flatForward = GetFlatForward();
        Vector3 flatRight = Vector3.Cross(Vector3.up, flatForward);

        Vector3 turnOffset = flatRight * turnInput * cameraOffsetOnTurn;
        Vector3 basePosition = carTransform.position - flatForward * cameraDistance + Vector3.up * cameraHeight;
        Vector3 desiredPosition = basePosition + turnOffset;

        float dynamicFollowSpeed = cameraFollowSpeed * (1f - currentCameraLag * 0.3f);

        carCamera.transform.position = Vector3.SmoothDamp(
            carCamera.transform.position,
            desiredPosition,
            ref cameraVelocity,
            1f / dynamicFollowSpeed
        );

        UpdateLookAt();
    }

    // Smoothly point the camera at the car, damping the vertical bounce from bumps.
    void UpdateLookAt()
    {
        Vector3 desiredLook = carTransform.position + Vector3.up * cameraLookHeight;
        smoothedLookTarget = Vector3.SmoothDamp(
            smoothedLookTarget, desiredLook, ref cameraLookVelocity, cameraLookSmoothTime);
        carCamera.transform.LookAt(smoothedLookTarget);
    }

    // Car forward projected onto the horizontal plane, so pitch/roll never tilts the camera.
    Vector3 GetFlatForward()
    {
        Vector3 flat = Vector3.ProjectOnPlane(carTransform.forward, Vector3.up);
        if (flat.sqrMagnitude < 0.001f)
            flat = Vector3.ProjectOnPlane(carTransform.up, Vector3.up); // car nose pointing straight up/down
        return flat.normalized;
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

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayEnvironmentHit(currentSpeed);

            // If the car is going fast enough, smash through the object
            if (currentSpeed > maxSpeed * 0.5f)
            {
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Obsticles"))
        {
            // Obstacles only play the hit sound — no damage, never destroyed.
            if (currentSpeed < 5f) return;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayEnvironmentHit(currentSpeed);
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
