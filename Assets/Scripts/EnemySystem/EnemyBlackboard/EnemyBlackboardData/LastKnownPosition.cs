using System.Collections;
using UnityEngine;

namespace EnemyBT.BlackboardData
{
    public class LastKnownPosition
    {
       public Vector3 lastRoomPosition {get; private set;}

        public Vector2Int lastRoomGridPosition {get; private set;}
        private float timeToTraverseRoom; // time to traverse room for player
        private int maxRoomReset; // reset at this many rooms traversed in time
        // Coroutine
        private IEnumerator knownTimer;
        public bool isLocationKnown {get; private set;}

        
        public LastKnownPosition(float timeTraversal, int maxRoom)
        {
            isLocationKnown = false;
            lastRoomPosition = Vector3.zero;
            lastRoomGridPosition = Vector2Int.zero;
            timeToTraverseRoom = timeTraversal;
            maxRoomReset = maxRoom;
        }

        // Sets the known position and kicks off the timer (need coroutine manager)
        public void SetKnownPosition(Vector3 position, Vector2Int gridPosition)
        {
            lastRoomPosition = position;
            lastRoomGridPosition = gridPosition;
            StartKnownTimer();
        }

        // Starts the timer, and resets it if started again
        private void StartKnownTimer()
        {
            if (isLocationKnown) {
                StopKnownTimer();
            }
            isLocationKnown = true;
            knownTimer = KnownLocationTimer();
            CoroutineManager.Instance.Run(knownTimer);
        }

        private void StopKnownTimer()
        {
            if (!isLocationKnown || CoroutineManager.Instance == null) return;
            CoroutineManager.Instance.Stop(knownTimer);
            knownTimer = null;
            isLocationKnown = false;
        }

        private IEnumerator KnownLocationTimer()
        {
            int roomsTraversed = 0;
            while (roomsTraversed < maxRoomReset)
            {
                yield return timeToTraverseRoom;
                roomsTraversed++;
            }
            StopKnownTimer();
        }
    }
}
