using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LayoutGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] int gridWidth = 20;
    [SerializeField] int gridHeight = 20;

    [Header("Room Placement")]
    [SerializeField] List<RoomData> availableRooms = new List<RoomData>();
    [SerializeField] int targetRoomCount = 10;
    [SerializeField] int maxPlacementAttempts = 100;
    [SerializeField] int roomSpacing = 1;

    [Header("Hallway Connections")]
    [SerializeField] int maxExtraConnections = 3;
    [SerializeField] int minConnectionLength = 3;

    [Header("Model Placement")]
    [SerializeField] ModelPlacementManager modelPlacementManager;

    GridCell[,] grid;

    //For model placement script to access
    public GridCell[,] Grid => grid;
    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;

    int nextRoomID = 0;

    List<Vector2Int> connectedDoors = new List<Vector2Int>();
    List<Vector2Int> attemptedDoors = new List<Vector2Int>();

    // For hallway placement algorithm, deters hallway turns unlessneccesary
    private int turnCost = 4;

    Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    private void Start()
    {
        GenerateLayout(); //CHANGE LATER. FOR TESTING PURPOSES, IN FUTURE THIS CALL WILL HAPPEN WHEN GENERATION IS NEEDED, NOT ON SCENE START
    }

    private void GenerateLayout()
    {
        CreateGrid();
        GenerateRooms();
        GenerateHallways();
        ConnectHallways();

        modelPlacementManager.AnalyzeLayout(grid, gridWidth, gridHeight);
        modelPlacementManager.PlaceRooms(grid, gridWidth, gridHeight);
        Debug.Log(EventBus.Instance.GetHashCode());
        EventBus.Instance.DoProceduralDone();
        Debug.Log("Done Procedural Generation Layout");
    }

    //Initializes grid to build layout off of. Uses gridWidth/gridHeight to generate size
    void CreateGrid()
    {
        grid = new GridCell[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = new GridCell();
                grid[x, y].gridPosition = new Vector2Int(x, y);
            }
        }
    }

    void PlaceStartingRoom()
    {
        //Places room at (0, gridHeight/2) on the grid
        Vector2Int position = new Vector2Int(0, gridHeight/2);

        // Make sure the position is inside the grid.
        if (position.x >= gridWidth || position.y >= gridHeight)
        {
            //Debug.LogWarning("Starting room position is outside the grid!");
            return;
        }

        GridCell cell = grid[position.x, position.y];

        cell.isOccupied = true;
        cell.roomID = nextRoomID;
        cell.tileType = TileType.Room;
        cell.isDoor = true;

        nextRoomID++;
    }

    // Attempts to place a room at the specified grid position.
    bool TryPlaceRoom(RoomData roomData, Vector2Int origin)
    {
        //Checks to make sure room exists
        if (roomData == null || roomData.tiles == null || roomData.width <= 0 || roomData.height <= 0 || roomData.tiles.Count == 0)
        {
            return false;
        }

        HashSet<Vector2Int> checkedPositions = new HashSet<Vector2Int>();

        // Check every tile before placing anything.
        foreach (RoomTileData tile in roomData.tiles)
        {
            Vector2Int position = origin + tile.position;

            // Make sure the tile is inside its room design.
            if (tile.position.x < 0 || tile.position.x >= roomData.width || tile.position.y < 0 || tile.position.y >= roomData.height)
            {
                return false;
            }

            // Make sure the tile is inside the main grid, 1 tile small than the grid so that no doors are blocked by the edge
            if (position.x < 1 || position.x >= gridWidth -1 || position.y < 1 || position.y >= gridHeight - 1)
            {
                return false;
            }

            // Prevent overlapping tiles in the room design.
            if (!checkedPositions.Add(position))
            {
                return false;
            }

            // Check the surrounding area for existing rooms, prevent overlapping
            for (int x = -roomSpacing; x <= roomSpacing; x++)
            {
                for (int y = -roomSpacing; y <= roomSpacing; y++)
                {
                    int checkX = position.x + x;
                    int checkY = position.y + y;

                    // Ignore positions outside the grid.
                    if (checkX < 0 || checkX >= gridWidth || checkY < 0 || checkY >= gridHeight)
                    {
                        continue;
                    }

                    if (grid[checkX, checkY].isOccupied)
                    {
                        return false;
                    }
                }
            }
        }

        // Assign a unique ID to this room.
        int roomID = nextRoomID;

        // Copy every individual room tile into the main grid.
        foreach (RoomTileData tile in roomData.tiles)
        {
            Vector2Int position = origin + tile.position;

            GridCell cell = grid[position.x, position.y];

            cell.isOccupied = true;
            cell.roomID = roomID;
            cell.tileType = TileType.Room;
            cell.isDoor = tile.isDoor;
        }

        nextRoomID++;

        return true;
    }

    //Generate rooms on the grid from the available rooms list
    void GenerateRooms()
    {
        PlaceStartingRoom();
        
        if (availableRooms.Count == 0)
        {
            Debug.LogWarning("No room designs assigned!");
            return;
        }

        int placedRooms = 0;

        for (int i = 0; i < maxPlacementAttempts; i++)
        {
            if (placedRooms >= targetRoomCount)
            {
                break;
            }

            // Randomly select a room design.
            RoomData selectedRoom = availableRooms[Random.Range(0, availableRooms.Count)];

            if (selectedRoom == null)
            {
                continue;
            }

            // Skip rooms that are larger than the grid.
            if (selectedRoom.width > gridWidth || selectedRoom.height > gridHeight)
            {
                continue;
            }

            // Randomly select a position where the room's entire design fits inside the main grid.
            Vector2Int randomPosition = new Vector2Int(Random.Range(0, gridWidth - selectedRoom.width + 1), Random.Range(0, gridHeight - selectedRoom.height + 1));

            // Attempt placement.
            if (TryPlaceRoom(selectedRoom, randomPosition))
            {
                placedRooms++;
            }
        }
    }

    //Finds open doors for the hallway generation
    Vector2Int FindFirstUnconnectedDoor()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GridCell cell = grid[x, y];

                //If the cell it finds is a door and it is open, return that cell position
                if (cell.isDoor && !connectedDoors.Contains(cell.gridPosition) && !attemptedDoors.Contains(cell.gridPosition))
                {
                    return cell.gridPosition;
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    //Finds the closest door for hallway generation to ensure hallways are a shot as possible
    List<Vector2Int> FindClosestDoor(Vector2Int startPosition)
    {
        List<PathNode> openSet = new List<PathNode>();
        Dictionary<(Vector2Int, Vector2Int), int> visited = new Dictionary<(Vector2Int, Vector2Int), int>();

        int startRoomID = grid[startPosition.x, startPosition.y].roomID;
        PathNode startNode = new PathNode(startPosition, Vector2Int.zero, 0, null);

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Find node with lowest cost.
            int lowestIndex = 0;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].cost < openSet[lowestIndex].cost)
                {
                    lowestIndex = i;
                }
            }

            PathNode current = openSet[lowestIndex];
            openSet.RemoveAt(lowestIndex);

            var currentKey = (current.position, current.direction);

            if (visited.ContainsKey(currentKey) && visited[currentKey] <= current.cost)
            {
                continue;
            }

            visited[currentKey] = current.cost;

            GridCell currentCell = grid[current.position.x, current.position.y];

            // Check whether we reached another room's door.
            if (current.position != startPosition && currentCell.isDoor)
            {
                // Never connect to a door in the same room.
                if (currentCell.roomID == startRoomID)
                {
                    continue;
                }

                // Never connect to an already connected door.
                if (connectedDoors.Contains(current.position))
                {
                    continue;
                }

                return ReconstructPath(current);
            }

            // Explore neighboring cells.
            foreach (Vector2Int direction in directions)
            {
                Vector2Int nextPosition = current.position + direction;

                // Stay inside grid.
                if (nextPosition.x < 0 || nextPosition.x >= gridWidth || nextPosition.y < 0 || nextPosition.y >= gridHeight)
                {
                    continue;
                }

                GridCell nextCell = grid[nextPosition.x, nextPosition.y];

                // Cannot enter another door belonging to the starting room.
                if (nextCell.isDoor && nextCell.roomID == startRoomID)
                {
                    continue;
                }

                // Cannot travel through room interiors.
                if (nextCell.tileType == TileType.Room && !nextCell.isDoor)
                {
                    continue;
                }

                if (IsParallelHallwayNearby(nextPosition, direction))
                {
                    continue;
                }

                // Cannot travel through existing hallways.
                if (nextCell.tileType == TileType.Hallway)
                {
                    continue;
                }

                // Calculate movement cost.
                int movementCost = 1;

                if (current.direction != Vector2Int.zero && current.direction != direction)
                {
                    movementCost += turnCost;
                }

                int newCost = current.cost + movementCost;

                PathNode nextNode = new PathNode(nextPosition, direction, newCost, current);

                openSet.Add(nextNode);
            }
        }

        Debug.LogWarning("No available door could be reached!");

        return null;
    }

    //Retraces pathfinding result to find every cell the hallway needs to be in
    List<Vector2Int> ReconstructPath(PathNode node)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (node != null)
        {
            path.Add(node.position);
            node = node.previous;
        }

        path.Reverse();

        return path;
    }

    //Creates the hallway design after pathfinding is finished
    void CreateHallway(List<Vector2Int> path)
    {
        if (path == null || path.Count < 2)
        {
            Debug.LogWarning("Invalid hallway path!");
            return;
        }

        // Convert empty cells into hallway tiles.
        foreach (Vector2Int position in path)
        {
            GridCell cell = grid[position.x, position.y];

            // Don't overwrite room tiles or doors.
            if (cell.tileType == TileType.Empty)
            {
                cell.isOccupied = true;
                cell.tileType = TileType.Hallway;
            }
        }

        // Mark both endpoint doors as connected.
        Vector2Int startDoor = path[0];
        Vector2Int endDoor = path[path.Count - 1];

        if (!connectedDoors.Contains(startDoor))
        {
            connectedDoors.Add(startDoor);
        }

        if (!connectedDoors.Contains(endDoor))
        {
            connectedDoors.Add(endDoor);
        }
    }

    //Creates all of the hallways after room generation has completed
    void GenerateHallways()
    {
        while (true)
        {
            //Looks for the first unconnected door, starts generation from there until all doors are connected
            Vector2Int startDoor = FindFirstUnconnectedDoor();

            if (startDoor.x == -1)
            {
                break;
            }

            List<Vector2Int> path = FindClosestDoor(startDoor);

            if (path == null)
            {
                attemptedDoors.Add(startDoor);
                continue;
            }

            int startRoomID = grid[path[0].x, path[0].y].roomID;
            Vector2Int endPosition = path[path.Count - 1];
            int endRoomID = grid[endPosition.x, endPosition.y].roomID;

            // Skip connections within the same room.
            if (startRoomID == endRoomID)
            {
                attemptedDoors.Add(startDoor);
                continue;
            }

            CreateHallway(path);
        }
    }

    //Connects the hallways together after initial hallway generation is done - Provides interconnectivity
    private void ConnectHallways()
    {
        int connectionsMade = 0;
        int attempts = 0;
        int maxAttempts = maxExtraConnections * 15;

        while (connectionsMade < maxExtraConnections && attempts < maxAttempts)
        {
            attempts++;

            // Re-detect networks because each successful connection merges them.
            List<List<Vector2Int>> hallwayNetworks = GetHallwayNetworks();

            if (hallwayNetworks.Count < 2) break;

            // Pick two different networks at random.
            int firstIndex = Random.Range(0, hallwayNetworks.Count);
            int secondIndex = Random.Range(0, hallwayNetworks.Count - 1);

            if (secondIndex >= firstIndex) secondIndex++;

            List<Vector2Int> firstNetwork = hallwayNetworks[firstIndex];
            List<Vector2Int> secondNetwork = hallwayNetworks[secondIndex];

            // Chance to attempt this connection.
            //if (Random.value > connectionChance) continue;

            List<Vector2Int> path = FindConnectionPath(firstNetwork, secondNetwork);

            if (path == null) continue;

            // Count only the new empty hallway tiles.
            int newTiles = 0;

            foreach (Vector2Int position in path)
            {
                if (grid[position.x, position.y].tileType == TileType.Empty)
                {
                    newTiles++;
                }
            }

            if (newTiles < minConnectionLength) continue;

            // Create the new hallway.
            foreach (Vector2Int position in path)
            {
                GridCell cell = grid[position.x, position.y];

                if (cell.tileType == TileType.Empty)
                {
                    cell.tileType = TileType.Hallway;
                    cell.isOccupied = true;
                }
            }

            connectionsMade++;
        }
    }

    //Finds all of the hallways, and returns them, helper for the hallway interconnectivity
    List<List<Vector2Int>> GetHallwayNetworks()
    {
        List<List<Vector2Int>> networks = new List<List<Vector2Int>>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2Int start = new Vector2Int(x, y);

                //Skips the cell if its not a hallway or has the start
                if (grid[x, y].tileType != TileType.Hallway || visited.Contains(start))
                {
                    continue;
                }

                List<Vector2Int> network = new List<Vector2Int>();
                Queue<Vector2Int> queue = new Queue<Vector2Int>();

                queue.Enqueue(start);
                visited.Add(start);

                //Loops through all the hallways to see the networks, distinguishes networks from another
                while (queue.Count > 0)
                {
                    Vector2Int current = queue.Dequeue();
                    network.Add(current);

                    foreach (Vector2Int direction in directions)
                    {
                        Vector2Int neighbor = current + direction;

                        //Skip if the hallway is outside of the grid
                        if (neighbor.x < 0 || neighbor.x >= gridWidth || neighbor.y < 0 || neighbor.y >= gridHeight) continue;

                        //Skip if the neighbor has already been visited
                        if (visited.Contains(neighbor)) continue;

                        //Skip if the neighbor is not a hallway
                        if (grid[neighbor.x, neighbor.y].tileType != TileType.Hallway) continue;

                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }

                networks.Add(network);
            }
        }

        return networks;
    }

    //Finds a path to connect hallways together
    List<Vector2Int> FindConnectionPath(List<Vector2Int> firstNetwork, List<Vector2Int> secondNetwork)
    {
        List<PathNode> openSet = new List<PathNode>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        HashSet<Vector2Int> targetTiles = new HashSet<Vector2Int>(secondNetwork);

        // Start searching from the entire first hallway network.
        foreach (Vector2Int start in firstNetwork)
        {
            openSet.Add(new PathNode(start, Vector2Int.zero, 0, null));
        }

        while (openSet.Count > 0)
        {
            // Find the lowest-cost node.
            int lowestIndex = 0;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].cost < openSet[lowestIndex].cost)
                {
                    lowestIndex = i;
                }
            }

            PathNode current = openSet[lowestIndex];
            openSet.RemoveAt(lowestIndex);

            if (!visited.Add(current.position)) continue;

            foreach (Vector2Int direction in directions)
            {
                Vector2Int nextPosition = current.position + direction;

                // Stay inside the grid.
                if (nextPosition.x < 1 || nextPosition.x >= gridWidth - 1 || nextPosition.y < 1 || nextPosition.y >= gridHeight - 1) continue;
                

                GridCell nextCell = grid[nextPosition.x, nextPosition.y];

                // If we reached the target network, reconstruct the path.
                if (targetTiles.Contains(nextPosition))
                {
                    List<Vector2Int> path = ReconstructPath(current);
                    return path;
                }

                if (visited.Contains(nextPosition)) continue;

                // Only carve through empty cells.
                if (nextCell.tileType != TileType.Empty) continue;

                // Prevent extra connections from running parallel beside existing hallways.
                if (IsParallelHallwayNearby(nextPosition, direction)) continue;

                PathNode nextNode = new PathNode(nextPosition, direction, current.cost + 1, current);

                openSet.Add(nextNode);
            }
        }

        // No valid path found.
        return null;
    }

    //Cleanup so that hallways don't spawn parallel, makes the map more cramped and less open
    bool IsParallelHallwayNearby(Vector2Int position, Vector2Int direction)
    {
        Vector2Int perpendicular1;
        Vector2Int perpendicular2;

        if (direction.x != 0) // Moving horizontally
        {
            perpendicular1 = Vector2Int.up;
            perpendicular2 = Vector2Int.down;
        }
        else // Moving vertically
        {
            perpendicular1 = Vector2Int.left;
            perpendicular2 = Vector2Int.right;
        }

        Vector2Int check1 = position + perpendicular1;
        Vector2Int check2 = position + perpendicular2;

        if (check1.x >= 0 && check1.x < gridWidth &&
            check1.y >= 0 && check1.y < gridHeight)
        {
            if (grid[check1.x, check1.y].tileType == TileType.Hallway)
                return true;
        }

        if (check2.x >= 0 && check2.x < gridWidth &&
            check2.y >= 0 && check2.y < gridHeight)
        {
            if (grid[check2.x, check2.y].tileType == TileType.Hallway)
                return true;
        }

        return false;
    }

    //Helper function in editor mode to see what the grid looks like in data, not in in-game representation
    void OnDrawGizmos()
    {
        if (grid == null)
            return;

        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 10;
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.black;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * 2, 0, y * 2);

                if (grid[x, y].isOccupied)
                {
                    if (grid[x, y].isDoor) Gizmos.color = Color.green;
                    
                    else if (grid[x, y].tileType == TileType.Hallway) Gizmos.color = Color.yellow;
                    
                    else Gizmos.color = Color.cyan;

                    Gizmos.DrawCube(position, new Vector3(2 * 0.9f, 0.1f, 2 * 0.9f));
                }

                Gizmos.color = Color.gray;

                Gizmos.DrawWireCube(position, new Vector3(2, 0.05f, 2));
                
                Vector3 labelPosition = position + Vector3.up * 0.15f;

                Handles.Label(labelPosition, $"({x},{y})", labelStyle);   
            }
        }
    }
}

//Seperate class to help with the pathfinding
class PathNode
{
    public Vector2Int position;
    public Vector2Int direction;
    public int cost;

    public PathNode previous;

    public PathNode(Vector2Int position, Vector2Int direction, int cost, PathNode previous)
    {
        this.position = position;
        this.direction = direction;
        this.cost = cost;
        this.previous = previous;
    }
}