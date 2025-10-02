using UnityEngine;

public enum UpgradeType
{
  Speed,
  Acceleration,
  Health,
  Repair,
  Steering,
  Brake,
  Nitro,
  MaxXPSpawn,
  XPValue,
  Shield,
  ScoreMultiplier
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Scriptable Objects/Upgrade")]
public class UpgradeData : ScriptableObject
{
  public string header;
  public Sprite icon;
  public string description;
  public UpgradeType type;
}
