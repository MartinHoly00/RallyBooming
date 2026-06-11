using UnityEngine;

[CreateAssetMenu(fileName = "MapData_", menuName = "Maps/MapData")]
public class MapData : ScriptableObject
{
    public string mapId;
    public string sceneName;
    public string mapName;
    public Sprite mapImage;
}
