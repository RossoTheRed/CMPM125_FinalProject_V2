using UnityEngine;
using EnemyBT.BlackboardData;
[System.Serializable]
public class EnemyBlackboard
{
    // Core components
    public EnemyFSM enemyFSM {get ; private set;}
    public EnemyDirectorFSM enemyDirectorFSM {get ; private set;}
    public bool isBlackboardActive {get; private set;} = false;
    // Data Classes
    public PredictedRoom predictedRoom {get ; private set; }
    public LastKnownPosition lastKnownPosition {get ; private set; }
    public GridData gridData {get ; private set;}
    // Settings
    [SerializeField] private float roomTraversalTime = 4f;
    [SerializeField] private int roomTravelReset = 2;
    [SerializeField] private float roomSpacing = 0f;
    // General Info
    public bool _isValidPrediction => predictedRoom.isValidPrediction;
    public bool _isLocationKnown => lastKnownPosition.isLocationKnown;


    /// <summary>
    /// General Setup Process
    /// </summary>
    public EnemyBlackboard()
    {
        predictedRoom = new PredictedRoom();
        lastKnownPosition = new LastKnownPosition(roomTraversalTime, roomTravelReset);
    
    }

    public void SetEnemyFSM(EnemyFSM _FSM)
    {
        if (enemyFSM) return;
        enemyFSM = _FSM;
        if (enemyDirectorFSM) {
            Debug.Log("Enemy Blackboard Complete");
            isBlackboardActive = true;
        }
    }

    public void SetEnemyDirector(EnemyDirectorFSM _FSM)
    {
        if (enemyDirectorFSM) return;
        enemyDirectorFSM = _FSM;
        if (enemyFSM) {
            Debug.Log("Enemy Blackboard Complete");
            isBlackboardActive = true;
        }
    }
    
    public void SetGrid(GridCell[,] grid, float roomSize = 0f)
    {
        float roomSpace = roomSize;
        if (roomSpace == 0f) roomSpace = roomSpacing;
        gridData = new GridData(grid, roomSpace);
        this.roomSpacing = roomSize;
        Debug.Log("Setup Grid Complete");
    }
    /// <summary>
    /// Data Send
    /// </summary>
    public void SetLastKnownPosition(Vector3 foundPosition, Vector2Int foundGridPosition)
    {
        lastKnownPosition.SetKnownPosition(foundPosition, foundGridPosition);
    }
    public void SetPredictedRoom(Vector3 position, Vector2Int gridPosition)
    {
        predictedRoom.isValidPrediction = true;
        predictedRoom.predictedRoomPosition = position;
        predictedRoom.predictedRoomGridPosition = gridPosition;

    }
    public void InvalidatePredictedRoom()
    {
        predictedRoom.isValidPrediction = false;
        predictedRoom.predictedRoomPosition = Vector3.zero;
        predictedRoom.predictedRoomGridPosition = Vector2Int.zero;

    }

    /// <summary>
    /// Data Request
    /// </summary>
    public Vector3 GetLastKnownPosition()
    {
        return lastKnownPosition.lastRoomPosition;
    }
    public Vector2Int GetLastKnownGridLocation()
    {
        return lastKnownPosition.lastRoomGridPosition;
    }
    public Vector3 GetPredictedRoomPosition()
    {
        return predictedRoom.predictedRoomPosition;
    }
    public Vector2Int GetPredictedRoomGridPosition()
    {
        return predictedRoom.predictedRoomGridPosition;
    }
    public float GetRoomSize()
    {
        return roomSpacing;
    }
    
}
