using UnityEngine;

[CreateAssetMenu(fileName = "CarData_", menuName = "Cars/CarData")]
public class CarData : ScriptableObject
{
  public string carId;
  public GameObject bodyPrefab;
  [Header("Wheel Prefabs")]
  public GameObject frontLeftWheelPrefab;
  public GameObject frontRightWheelPrefab;
  public GameObject rearLeftWheelPrefab;
  public GameObject rearRightWheelPrefab;

  [Header("Wheel Meshes")]
  public Transform frontLeftWheelMesh;
  public Transform frontRightWheelMesh;
  public Transform rearLeftWheelMesh;
  public Transform rearRightWheelMesh;

  [Header("Stats")]
  public float mass = 1000f;
  public float acceleration = 250f;
  public float maxSpeed = 30f;
  public float steeringSpeed = 100f;
  public float brakeForce = 1f;
  public float brakeDrag = 100f;
  public float health = 100f;
}
