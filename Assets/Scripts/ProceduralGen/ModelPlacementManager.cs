using UnityEngine;

public class ModelPlacementManager : MonoBehaviour
{
    [Header("Hallway Prefabs")]
    [SerializeField] GameObject straightNorthSouth;
    [SerializeField] GameObject straightEastWest;

    [SerializeField] GameObject elbowNorthEast;
    [SerializeField] GameObject elbowEastSouth;
    [SerializeField] GameObject elbowSouthWest;
    [SerializeField] GameObject elbowWestNorth;

    [SerializeField] GameObject tMissingNorth;
    [SerializeField] GameObject tMissingEast;
    [SerializeField] GameObject tMissingSouth;
    [SerializeField] GameObject tMissingWest;

    [SerializeField] GameObject crossPrefab;

    [Header("Room Prefabs")]
    [SerializeField] GameObject roomMiddle;

    [SerializeField] GameObject sideNorth;
    [SerializeField] GameObject sideEast;
    [SerializeField] GameObject sideSouth;
    [SerializeField] GameObject sideWest;

    [SerializeField] GameObject cornerNorthEast;
    [SerializeField] GameObject cornerNorthWest;
    [SerializeField] GameObject cornerSouthEast;
    [SerializeField] GameObject cornerSouthWest;

    [SerializeField] GameObject entranceNorth;
    [SerializeField] GameObject entranceEast;
    [SerializeField] GameObject entranceSouth;
    [SerializeField] GameObject entranceWest;

    [Header("Placement")]
    [SerializeField] float gridSpacing = 4f;
    [SerializeField] Transform generatedModelsParent;

    //Loops through the grid to see what see what types of models should be placed
    public void AnalyzeLayout(GridCell[,] grid, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = grid[x, y];

                if (cell.tileType == TileType.Hallway)
                {
                    TileConnections connections = GetConnections(grid, x, y, width, height);
                    AnalyzeHallway(cell, connections);
                }
            }
        }
    }

    //Checks to see which directions a room/hallway is connected with another
    TileConnections GetConnections(GridCell[,] grid, int x, int y, int width, int height)
    {
        TileConnections connections = TileConnections.None;

        if (IsConnected(grid, x, y + 1, width, height))
        {
            connections |= TileConnections.North;
        }
        if (IsConnected(grid, x + 1, y, width, height))
        {
            connections |= TileConnections.East;
        }
        if (IsConnected(grid, x, y - 1, width, height))
        {
            connections |= TileConnections.South;
        }
        if (IsConnected(grid, x - 1, y, width, height))
        {
            connections |= TileConnections.West;
        }
        return connections;
    }

    //Helper function to determine if a room is connected to another
    bool IsConnected(GridCell[,] grid, int x, int y, int width, int height)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        GridCell neighbor = grid[x, y];

        // Hallway cells connect to hallways and room doors.
        return neighbor.tileType == TileType.Hallway || (neighbor.tileType == TileType.Room && neighbor.isDoor);
    }

    //Looks at a hallway, and determines what prefab it should be assigned 
    void AnalyzeHallway(GridCell cell, TileConnections connections)
    {
        GameObject prefab = null;

        // Straight
        if (connections == (TileConnections.North | TileConnections.South))
        {
            prefab = straightNorthSouth;
        }
        else if (connections == (TileConnections.East | TileConnections.West))
        {
            prefab = straightEastWest;
        }

        // Corners
        else if (connections == (TileConnections.North | TileConnections.East))
        {
            prefab = elbowNorthEast;
        }
        else if (connections == (TileConnections.East | TileConnections.South))
        {
            prefab = elbowEastSouth;
        }
        else if (connections == (TileConnections.South | TileConnections.West))
        {
            prefab = elbowSouthWest;
        }
        else if (connections == (TileConnections.West | TileConnections.North))
        {
            prefab = elbowWestNorth;
        }

        // T-Junctions (named by the missing direction)
        else if (connections == (TileConnections.East | TileConnections.South | TileConnections.West))
        {
            prefab = tMissingNorth;
        }
        else if (connections == (TileConnections.North | TileConnections.South | TileConnections.West))
        {
            prefab = tMissingEast;
        }
        else if (connections == (TileConnections.North | TileConnections.East | TileConnections.West))
        {
            prefab = tMissingSouth;
        }
        else if (connections == (TileConnections.North | TileConnections.East | TileConnections.South))
        {
            prefab = tMissingWest;
        }

        // Cross
        else if (connections == (TileConnections.North | TileConnections.East | TileConnections.South | TileConnections.West))
        {
            prefab = crossPrefab;
        }

        if (prefab == null)
        {
            Debug.LogWarning($"No hallway prefab assigned or unsupported connection at {cell.gridPosition}: {connections}");
            return;
        }

        Vector3 position = new Vector3(cell.gridPosition.x * gridSpacing, 0, cell.gridPosition.y * gridSpacing);

        GameObject spawned = Instantiate(prefab, position, prefab.transform.rotation, generatedModelsParent);

        spawned.name = $"{prefab.name}_{cell.gridPosition.x}_{cell.gridPosition.y}";
    }

    //Loops through the grid to find which cells should be placed as a room
    public void PlaceRooms(GridCell[,] grid, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = grid[x, y];

                if (cell.tileType != TileType.Room)
                    continue;

                GameObject prefab = GetRoomPrefab(grid, x, y, width, height);

                if (prefab == null)
                {
                    Debug.LogWarning($"No room prefab found at {cell.gridPosition}");
                    continue;
                }

                Vector3 position = new Vector3(x * gridSpacing, 0, y * gridSpacing);
                Instantiate(prefab, position, prefab.transform.rotation, generatedModelsParent);
            }
        }
    }

    //Gives a room the prefab necessary based on its connections to other rooms
    GameObject GetRoomPrefab(GridCell[,] grid, int x, int y, int width, int height)
    {
        GridCell cell = grid[x, y];

        bool northSameRoom = IsSameRoom(grid, x, y + 1, width, height, cell.roomID);
        bool eastSameRoom = IsSameRoom(grid, x + 1, y, width, height, cell.roomID);
        bool southSameRoom = IsSameRoom(grid, x, y - 1, width, height, cell.roomID);
        bool westSameRoom = IsSameRoom(grid, x - 1, y, width, height, cell.roomID);

        // Door tiles use the entrance prefab facing the connecting hallway.
        if (cell.isDoor)
        {
            if (IsHallway(grid, x, y + 1, width, height)) return entranceNorth;
            if (IsHallway(grid, x + 1, y, width, height)) return entranceEast;
            if (IsHallway(grid, x, y - 1, width, height)) return entranceSouth;
            if (IsHallway(grid, x - 1, y, width, height)) return entranceWest;
        }

        // Determine which sides of this room tile are exposed.
        bool north = !northSameRoom;
        bool east = !eastSameRoom;
        bool south = !southSameRoom;
        bool west = !westSameRoom;

        // Corners
        if (north && east && !south && !west) return cornerNorthEast;
        if (north && west && !south && !east) return cornerNorthWest;
        if (south && east && !north && !west) return cornerSouthEast;
        if (south && west && !north && !east) return cornerSouthWest;

        // Sides
        if (north && !east && !south && !west) return sideNorth;
        if (east && !north && !south && !west) return sideEast;
        if (south && !north && !east && !west) return sideSouth;
        if (west && !north && !east && !south) return sideWest;

        // Interior tile
        if (!north && !east && !south && !west) return roomMiddle;

        Debug.LogWarning($"Unsupported room shape at ({x},{y}). Using roomMiddle.");
        return roomMiddle;
    }

    //Helper function to determine if the room is the same as another
    bool IsSameRoom(GridCell[,] grid, int x, int y, int width, int height, int roomID)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        GridCell neighbor = grid[x, y];

        return neighbor.tileType == TileType.Room && neighbor.roomID == roomID;
    }

    //Helper function to retunr if a cell is a hallway
    bool IsHallway(GridCell[,] grid, int x, int y, int width, int height)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return false;

        return grid[x, y].tileType == TileType.Hallway;
    }
}
