using UnityEngine;

namespace EnemyBT.BlackboardData
{
    public class PredictedRoom
    {
        public Vector3 predictedRoomPosition;
        public Vector2Int predictedRoomGridPosition;
        public bool isValidPrediction;
        public PredictedRoom()
        {
            isValidPrediction = false;
            predictedRoomPosition = Vector3.zero;
            predictedRoomGridPosition = Vector2Int.zero;
        }
    }
}