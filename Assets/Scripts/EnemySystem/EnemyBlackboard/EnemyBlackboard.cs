using UnityEngine;
using EnemyBT.BlackboardData;
public class EnemyBlackboard
{
    // Core components
    public EnemyFSM enemyFSM {get ; private set;}
    public EnemyDirectorFSM enemyDirectorFSM {get ; private set;}
    public bool isBlackboardActive = false;
    // Data Classes
    public PredictedRoom predictedRoom {get ; private set; }
    public LastKnownPosition lastKnownPosition {get ; private set; }
    // Settings
    [SerializeField] private float roomTraversalTime = 4f;
    [SerializeField] private int roomTravelReset = 2;
    [SerializeField] private float roomSize = 2f;
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
        if (enemyDirectorFSM) isBlackboardActive = true;
    }

    public void SetEnemyDirector(EnemyDirectorFSM _FSM)
    {
        if (enemyDirectorFSM) return;
        enemyDirectorFSM = _FSM;
        if (enemyFSM) isBlackboardActive = true;
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
        return roomSize;
    }
    
}
