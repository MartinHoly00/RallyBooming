using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "Maps/MapDatabase")]
public class MapDatabase : ScriptableObject
{
    public List<MapData> maps = new List<MapData>();

    public MapData GetById(string id)
    {
        return maps.Find(m => m != null && m.mapId == id);
    }
}
