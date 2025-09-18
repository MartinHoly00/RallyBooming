using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Data")]
    public CarDatabase carDatabase;
    public string playerPrefsKey = "SelectedCarId";
    public string fallbackCarId = "";

    [Header("Spawn")]
    public Transform carSpawn;

    [Tooltip("Optional camera")]
    public Camera carCamera;

    [Header("Tags")]
    public string bodyTag = "CarBody";
    public string frontLeftWheelMeshTag = "WheelMesh_FL";
    public string frontRightWheelMeshTag = "WheelMesh_FR";
    public string rearLeftWheelMeshTag = "WheelMesh_RL";
    public string rearRightWheelMeshTag = "WheelMesh_RR";

    public string frontLeftWheelColliderTag = "WheelCollider_FL";
    public string frontRightWheelColliderTag = "WheelCollider_FR";
    public string rearLeftWheelColliderTag = "WheelCollider_RL";
    public string rearRightWheelColliderTag = "WheelCollider_RR";

    [Header("Runtime state")]
    public GameObject currentCarInstance;

    [Header("Body mount")]
    public Transform bodyMount;

    void Start()
    {
        if (carCamera == null) carCamera = Camera.main;
        if (carDatabase == null) return;
        if (carSpawn == null) return;

        SpawnSelectedCar();
    }

    public void SpawnSelectedCar()
    {
        string id = PlayerPrefs.GetString(playerPrefsKey, fallbackCarId);
        CarData data = carDatabase.GetById(id);

        if (data == null)
        {
            if (carDatabase.cars.Count > 0)
            {
                data = carDatabase.cars[0];
            }
        }

        SpawnCarPrefab(data);
    }

    private void SpawnCarPrefab(CarData data)
    {
        if (currentCarInstance != null)
        {
            Destroy(currentCarInstance);
            currentCarInstance = null;
        }

        // instantiate
        Quaternion rot = carSpawn.rotation;
        Vector3 pos = carSpawn.position;
        GameObject inst = Instantiate(data.carPrefab, pos, rot, bodyMount != null ? bodyMount : carSpawn);
        currentCarInstance = inst;

        CarControl cc = inst.GetComponentInChildren<CarControl>(true) ?? inst.GetComponent<CarControl>();

        WireCarControl(cc, inst, data);
    }


    private void WireCarControl(CarControl cc, GameObject inst, CarData data)
    {
        cc.carTransform = inst.transform;

        Rigidbody rb = inst.GetComponent<Rigidbody>() ?? inst.GetComponentInChildren<Rigidbody>(true);
        if (rb != null)
        {
            cc.rb = rb;
            rb.mass = data.mass;
            cc.ApplyCarData(data);
        }

        // camera
        if (carCamera != null) cc.carCamera = carCamera;

        // health system if present
        var hs = inst.GetComponentInChildren<HealthSystem>(true);
        if (hs != null) cc.healthSystem = hs;

        // Assign numeric stats (do NOT replace visuals)
        cc.ApplyCarData(data);

        // Find wheel colliders & meshes (tries tags first then name-based heuristics, then fallback to ordering)
        cc.frontLeftWheel = FindWheelCollider(inst, frontLeftWheelColliderTag, new string[] { "frontleft", "front_left", "fl", "front left", "wheel_fl", "wheelfrontleft" });
        cc.frontRightWheel = FindWheelCollider(inst, frontRightWheelColliderTag, new string[] { "frontright", "front_right", "fr", "front right", "wheel_fr", "wheelfrontright" });
        cc.rearLeftWheel = FindWheelCollider(inst, rearLeftWheelColliderTag, new string[] { "rearleft", "rear_left", "rl", "rear left", "wheel_rl", "wheelrearleft" });
        cc.rearRightWheel = FindWheelCollider(inst, rearRightWheelColliderTag, new string[] { "rearright", "rear_right", "rr", "rear right", "wheel_rr", "wheelrearright" });

        cc.frontLeftWheelMesh = FindWheelMeshTransform(inst, frontLeftWheelMeshTag, new string[] { "frontleft", "front_left", "fl", "wheel_fl", "frontleft_wheel", "wheel_front_left" });
        cc.frontRightWheelMesh = FindWheelMeshTransform(inst, frontRightWheelMeshTag, new string[] { "frontright", "front_right", "fr", "wheel_fr", "frontright_wheel", "wheel_front_right" });
        cc.rearLeftWheelMesh = FindWheelMeshTransform(inst, rearLeftWheelMeshTag, new string[] { "rearleft", "rear_left", "rl", "wheel_rl", "rearleft_wheel", "wheel_rear_left" });
        cc.rearRightWheelMesh = FindWheelMeshTransform(inst, rearRightWheelMeshTag, new string[] { "rearright", "rear_right", "rr", "wheel_rr", "rearright_wheel", "wheel_rear_right" });

        MapWheelCollidersIfNeeded(inst, cc);
        MapWheelMeshesIfNeeded(inst, cc);

        if (cc.rb != null)
        {
            cc.ApplyCarData(data);
        }
    }

    private WheelCollider FindWheelCollider(GameObject root, string tag, string[] nameCandidates)
    {
        // try tag first (if provided)
        if (!string.IsNullOrEmpty(tag))
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                try
                {
                    if (t.gameObject.tag == tag)
                    {
                        var wc = t.GetComponent<WheelCollider>();
                        if (wc != null) return wc;
                    }
                }
                catch { }
            }
        }

        // try name candidates
        foreach (var cand in nameCandidates)
        {
            var found = FindTransformByNameContains(root.transform, cand);
            if (found != null)
            {
                var wc = found.GetComponent<WheelCollider>();
                if (wc != null) return wc;
            }
        }

        // fallback: find any WheelCollider anywhere (returns null if none)
        var any = root.GetComponentsInChildren<WheelCollider>(true);
        if (any != null && any.Length > 0) return any[0];

        return null;
    }

    private Transform FindWheelMeshTransform(GameObject root, string tag, string[] nameCandidates)
    {
        if (!string.IsNullOrEmpty(tag))
        {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
            {
                try
                {
                    if (t.gameObject.tag == tag)
                        return t;
                }
                catch { /* ignore */ }
            }
        }

        foreach (var cand in nameCandidates)
        {
            var found = FindTransformByNameContains(root.transform, cand);
            if (found != null) return found;
        }

        // fallback: first transform whose name contains "wheel"
        var all = root.GetComponentsInChildren<Transform>(true);
        foreach (var t in all)
        {
            if (t.name.ToLower().Contains("wheel")) return t;
        }

        return null;
    }

    private Transform FindTransformByNameContains(Transform root, string substr)
    {
        if (root == null || string.IsNullOrEmpty(substr)) return null;
        string s = substr.ToLower();
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.ToLower().Contains(s)) return t;
        }
        return null;
    }

    private void MapWheelCollidersIfNeeded(GameObject inst, CarControl cc)
    {
        var all = inst.GetComponentsInChildren<WheelCollider>(true);
        if (all == null || all.Length < 4) return;

        if (cc.frontLeftWheel != null && cc.frontRightWheel != null && cc.rearLeftWheel != null && cc.rearRightWheel != null) return;

        // compute local positions relative to car root
        var entries = new List<(WheelCollider wc, Vector3 localPos)>();
        foreach (var w in all)
            entries.Add((w, inst.transform.InverseTransformPoint(w.transform.position)));

        // sort by z (forward)
        var sortedByZ = entries.OrderByDescending(e => e.localPos.z).ToArray(); // front (higher z) come first
        var front = sortedByZ.Take(2).ToArray();
        var rear = sortedByZ.Skip(2).Take(2).ToArray();

        // within front/rear, use x sign for left/right
        (WheelCollider fl, WheelCollider fr) = PickLeftRight(front);
        (WheelCollider rl, WheelCollider rr) = PickLeftRight(rear);

        if (cc.frontLeftWheel == null) cc.frontLeftWheel = fl;
        if (cc.frontRightWheel == null) cc.frontRightWheel = fr;
        if (cc.rearLeftWheel == null) cc.rearLeftWheel = rl;
        if (cc.rearRightWheel == null) cc.rearRightWheel = rr;
    }

    private void MapWheelMeshesIfNeeded(GameObject inst, CarControl cc)
    {
        var all = inst.GetComponentsInChildren<Transform>(true).Where(t => t.name.ToLower().Contains("wheel")).ToArray();
        if (all == null || all.Length < 4) return;

        if (cc.frontLeftWheelMesh != null && cc.frontRightWheelMesh != null && cc.rearLeftWheelMesh != null && cc.rearRightWheelMesh != null) return;

        var entries = new List<(Transform t, Vector3 localPos)>();
        foreach (var t in all)
            entries.Add((t, inst.transform.InverseTransformPoint(t.position)));

        var sortedByZ = entries.OrderByDescending(e => e.localPos.z).ToArray();
        var front = sortedByZ.Take(2).ToArray();
        var rear = sortedByZ.Skip(2).Take(2).ToArray();

        var (fl, fr) = PickLeftRightTransform(front);
        var (rl, rr) = PickLeftRightTransform(rear);

        if (cc.frontLeftWheelMesh == null) cc.frontLeftWheelMesh = fl;
        if (cc.frontRightWheelMesh == null) cc.frontRightWheelMesh = fr;
        if (cc.rearLeftWheelMesh == null) cc.rearLeftWheelMesh = rl;
        if (cc.rearRightWheelMesh == null) cc.rearRightWheelMesh = rr;
    }

    private (WheelCollider left, WheelCollider right) PickLeftRight((WheelCollider wc, Vector3 localPos)[] arr)
    {
        if (arr == null || arr.Length == 0) return (null, null);
        if (arr.Length == 1) return (arr[0].wc, null);

        var ordered = arr.OrderBy(e => e.localPos.x).ToArray(); // left (more negative x) first
        return (ordered[0].wc, ordered.Length > 1 ? ordered[1].wc : null);
    }

    private (Transform left, Transform right) PickLeftRightTransform((Transform t, Vector3 localPos)[] arr)
    {
        if (arr == null || arr.Length == 0) return (null, null);
        if (arr.Length == 1) return (arr[0].t, null);

        var ordered = arr.OrderBy(e => e.localPos.x).ToArray();
        return (ordered[0].t, ordered.Length > 1 ? ordered[1].t : null);
    }
}
