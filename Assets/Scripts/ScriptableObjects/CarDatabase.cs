using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarDatabase", menuName = "Cars/CarDatabase")]
public class CarDatabase : ScriptableObject
{
    public List<CarData> cars = new List<CarData>();

    public CarData GetById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        return cars.Find(c => c != null && c.carId == id);
    }
}
