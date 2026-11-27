using UnityEngine;

public enum TileType
{
    Empty,
    Floor,
    Hallway,
    Room
}

public class GridCell
{
    public Vector2Int gridPosition;

    public int roomID = -1;

    public TileType tileType = TileType.Empty;

    public bool isOccupied;

    public bool isDoor;
}