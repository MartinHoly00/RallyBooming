using UnityEngine;

[CreateAssetMenu(fileName = "CarData_", menuName = "Cars/CarData")]
public class CarData : ScriptableObject
{
  public string carId;
  [Header("Prefab")]
  public GameObject carPrefab;

  [Header("Stats")]
  public float mass = 1000f;
  public float acceleration = 250f;
  public float maxSpeed = 30f;
  public float steeringSpeed = 100f;
  public float brakeForce = 1f;
  public float brakeDrag = 100f;
  public float health = 100f;
}
