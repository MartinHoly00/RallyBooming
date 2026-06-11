using UnityEngine;

public class MapSelector : MonoBehaviour
{
    public MapDatabase mapDatabase;

    public void SelectMap(string mapId)
    {
        PlayerPrefs.SetString("SelectedMapId", mapId);
        PlayerPrefs.Save();
    }

    public static string GetSelectedSceneNameFromPrefs(MapDatabase database)
    {
        if (database == null) return null;
        string savedId = PlayerPrefs.GetString("SelectedMapId", "");
        if (!string.IsNullOrEmpty(savedId))
        {
            MapData map = database.GetById(savedId);
            if (map != null) return map.sceneName;
        }
        // Fallback to first map
        if (database.maps.Count > 0 && database.maps[0] != null)
            return database.maps[0].sceneName;
        return null;
    }
}
