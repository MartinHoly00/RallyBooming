using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Scriptable Objects/UpgradeDatabase")]
public class UpgradeDatabase : ScriptableObject
{
  public List<UpgradeData> upgrades = new List<UpgradeData>();

  public UpgradeData GetByType(UpgradeType type)
  {
    return upgrades.Find(u => u != null && u.type == type);
  }
}
