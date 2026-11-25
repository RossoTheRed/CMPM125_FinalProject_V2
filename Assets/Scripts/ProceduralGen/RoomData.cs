using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Procedural Generation/Room")]
public class RoomData : ScriptableObject
{
    public int width;
    public int height;

    public List<RoomTileData> tiles = new List<RoomTileData>();
}

[System.Serializable]
public class RoomTileData
{
    public Vector2Int position;

    public bool isDoor;
}
