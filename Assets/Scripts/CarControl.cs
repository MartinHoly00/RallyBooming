using System;
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

    [Header("Body")]
    public Transform bodyMount;
    private GameObject currentBodyInstance;

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
    public float cameraLagOnTurn = 2f; // How much the camera lags during turns
    public float cameraOffsetOnTurn = 3f; // How far the camera moves to the outside of turns

    private Vector3 cameraVelocity;
    private float currentCameraLag = 0f;

    [Header("Braking")]
    public float brakeForce = 1f;   // Strength of braking
    public float brakeDrag = 100f;     // Extra drag when braking
    private float normalDrag;        // Default drag

    public HealthSystem healthSystem;

    private float deltaTime;
    private bool moveForward, moveBackward, turnLeft, turnRight;

    void Awake()
    {
        deltaTime = Time.fixedDeltaTime * 1000f;
    }

    void Start()
    {
        rb.mass = carMass;
        rb.linearDamping = 0.3f;
        rb.angularDamping = 3f;
        rb.centerOfMass = new Vector3(0, -0.5f, 0);

        normalDrag = rb.linearDamping; // Save default drag
    }

    void Update()
    {
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
        float move = Input.GetAxis("Vertical") * maxSpeed;
        float turn = Input.GetAxis("Horizontal");

        if (moveForward) move += 1f * maxSpeed;
        if (moveBackward) move += -1f * maxSpeed;
        if (turnLeft) turn += -1f;
        if (turnRight) turn += 1f;

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
            Vector3 brakeForceVector = -rb.linearVelocity.normalized * brakeForce;

            rb.AddForce(brakeForceVector, ForceMode.Acceleration);

            rb.linearDamping = Mathf.Lerp(rb.linearDamping, brakeDrag, Time.deltaTime * 2f);
        }
        else
        {
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

    public void PressForward() => moveForward = true;
    public void ReleaseForward() => moveForward = false;

    public void PressBackward() => moveBackward = true;
    public void ReleaseBackward() => moveBackward = false;

    public void PressLeft() => turnLeft = true;
    public void ReleaseLeft() => turnLeft = false;

    public void PressRight() => turnRight = true;
    public void ReleaseRight() => turnRight = false;

    public void ApplyCarData(CarData data)
    {
        if (data == null) return;

        carData = data;

        // Apply numeric stats
        acceleration = data.acceleration;
        maxSpeed = data.maxSpeed;
        steeringSpeed = data.steeringSpeed;
        brakeForce = data.brakeForce;
        brakeDrag = data.brakeDrag;
        carMass = data.mass;

        // set Rigidbody mass if available
        if (rb == null) rb = GetComponentInChildren<Rigidbody>();
        if (rb != null) rb.mass = carMass;

        // Apply health if HealthSystem exists
        if (healthSystem != null)
        {
            healthSystem.maxHealth = data.health;
            healthSystem.currentHealth = data.health;
        }

        // NEW: replace visual body and update wheel mount references if possible
        ReplaceBody(data.bodyPrefab);

        // Replace wheel visuals (safe: remove Rigidbodies & Colliders in the instantiated prefabs)
        ReplaceWheelMesh(frontLeftWheelMesh, data.frontLeftWheelPrefab, out frontLeftWheelMesh);
        ReplaceWheelMesh(frontRightWheelMesh, data.frontRightWheelPrefab, out frontRightWheelMesh);
        ReplaceWheelMesh(rearLeftWheelMesh, data.rearLeftWheelPrefab, out rearLeftWheelMesh);
        ReplaceWheelMesh(rearRightWheelMesh, data.rearRightWheelPrefab, out rearRightWheelMesh);
    }


    private void ReplaceWheelMesh(Transform mount, GameObject prefab, out Transform result)
    {
        result = mount;
        if (mount == null || prefab == null) return;

        // destroy existing children (visuals) under mount
        // copy children to array to avoid modifying collection during iteration
        Transform[] children = new Transform[mount.childCount];
        for (int i = 0; i < children.Length; i++) children[i] = mount.GetChild(i);
        for (int i = 0; i < children.Length; i++) Destroy(children[i].gameObject);

        // instantiate the wheel prefab as child of mount
        GameObject inst = Instantiate(prefab, mount);
        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.identity;
        inst.transform.localScale = Vector3.one;

        // remove physics components from visual prefab instance to avoid interference
        foreach (var r in inst.GetComponentsInChildren<Rigidbody>()) Destroy(r);
        foreach (var c in inst.GetComponentsInChildren<Collider>()) Destroy(c);

        // if the prefab has nested mesh root, we might want to point to that transform; use inst.transform as visual pivot
        result = inst.transform;
    }

    // NEW: helper - recursive find by name (case-insensitive)
    private Transform FindChildByNameRecursive(Transform parent, string name)
    {
        if (parent == null) return null;
        if (string.Equals(parent.name, name, StringComparison.OrdinalIgnoreCase))
            return parent;

        for (int i = 0; i < parent.childCount; i++)
        {
            var found = FindChildByNameRecursive(parent.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }

    // NEW: try multiple candidate names and return first match
    private Transform FindPreferredChild(Transform root, params string[] names)
    {
        if (root == null) return null;
        foreach (var n in names)
        {
            var t = FindChildByNameRecursive(root, n);
            if (t != null) return t;
        }
        return null;
    }

    // NEW: instantiate body prefab under bodyMount, remove physics, and try to locate wheel mounts inside it
    private void ReplaceBody(GameObject prefab)
    {
        // remove previous body visual
        if (currentBodyInstance != null) Destroy(currentBodyInstance);

        if (prefab == null || bodyMount == null) return;

        GameObject inst = Instantiate(prefab, bodyMount);
        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.identity;
        inst.transform.localScale = Vector3.one;

        // remove physics components from the visual prefab
        foreach (var rb in inst.GetComponentsInChildren<Rigidbody>()) Destroy(rb);
        //foreach (var col in inst.GetComponentsInChildren<Collider>()) Destroy(col);

        currentBodyInstance = inst;

        // Try to find wheel mount transforms in the newly instantiated body and assign them
        // Use a list of commonly used names (adjust if your prefabs use different naming)
        frontLeftWheelMesh = FindPreferredChild(inst.transform,
            "frontLeftWheelMesh", "FrontLeftWheelMesh", "FrontLeft", "front_left", "wheel_fl", "Wheel_FL") ?? frontLeftWheelMesh;
        frontRightWheelMesh = FindPreferredChild(inst.transform,
            "frontRightWheelMesh", "FrontRightWheelMesh", "FrontRight", "front_right", "wheel_fr", "Wheel_FR") ?? frontRightWheelMesh;
        rearLeftWheelMesh = FindPreferredChild(inst.transform,
            "rearLeftWheelMesh", "RearLeftWheelMesh", "RearLeft", "rear_left", "wheel_rl", "Wheel_RL") ?? rearLeftWheelMesh;
        rearRightWheelMesh = FindPreferredChild(inst.transform,
            "rearRightWheelMesh", "RearRightWheelMesh", "RearRight", "rear_right", "wheel_rr", "Wheel_RR") ?? rearRightWheelMesh;

        // Optional: if your body prefab contains transforms named exactly for wheel collider alignment,
        // you can also try to re-position wheel colliders here to match visual wheels. (Not included by default.)
    }
}

