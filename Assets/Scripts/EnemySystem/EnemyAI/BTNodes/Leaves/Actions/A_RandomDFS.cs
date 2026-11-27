using System.Collections.Generic;
using EnemyBT.Actions;
using UnityEngine;
using UnityEngine.AI;

public class RandomDFS : ActionNode
{
    private EnemyBlackboard enemyBlackboard;
    private int minStep;
    private int maxStep;
    
    public RandomDFS(EnemyBlackboard enemyBlackboard, int minStep, int maxStep) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
        m_eTickType = TickType.BH_BOTH;

        this.minStep = minStep;
        this.maxStep = maxStep;
    }
    
    sealed protected override Status Update() 
    {
        // because there is a fixed update, it should go and commit those actions
        return Status.BH_SUCCESS;
    }

    sealed protected override Status FixedUpdate()
    {
        Vector2Int newGridPos = RandomDFSProcess();
        Vector3 newPos = new Vector2(newGridPos.x, newGridPos.y);
        newPos.x *= enemyBlackboard.GetRoomSize();
        newPos.y *= enemyBlackboard.GetRoomSize();
        newPos = new Vector3(GameManager.Instance.bottomLeftCornerGridPosition.x + newPos.x, GameManager.Instance.bottomLeftCornerGridPosition.y + newPos.y, GameManager.Instance.bottomLeftCornerGridPosition.z);
        enemyBlackboard.SetPredictedRoom(newPos, newGridPos);
        Status status = Status.BH_SUCCESS;
        if (newPos == Vector3.zero) status = Status.BH_FAILURE;
        Debug.Log($"Nav Status: {status}");
        return status;
    }

    private Vector2Int RandomDFSProcess()
    {
        int randomStep = Random.Range(minStep, maxStep);
        int colSize = enemyBlackboard.gridData.gridArray.GetLength(0);
        int rowSize = enemyBlackboard.gridData.gridArray.GetLength(1); // height
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int currGridPos = enemyBlackboard.enemyFSM.GetCurrentRoom();
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
        };
        List<int> randomPool;
        
        Queue<Vector3Int> queueDFS = new Queue<Vector3Int>();
        // Queue stores x and y pos, and then the depth level
        queueDFS.Enqueue(new Vector3Int(currGridPos.x, currGridPos.y, 0));
        Vector3Int queuePos = Vector3Int.zero;

        while (queueDFS.Count > 0)
        {
            queuePos = queueDFS.Dequeue();
            if (queuePos.z >= randomStep) break;
            randomPool = SetupPooledDirections();
            bool queued = false;
            while (randomPool.Count > 0)
            {
                int randomIndex = Random.Range(0, randomPool.Count);
                int randomPoolValue = randomPool[randomIndex];
                Vector2Int newPos;
                newPos = directions[randomPoolValue];
                newPos += currGridPos;
                if (newPos.x >= 0 && newPos.x < rowSize && newPos.y >= 0 && newPos.y < colSize && !visited.Contains(newPos)) 
                {
                    if (enemyBlackboard.gridData.gridArray[newPos.x, newPos.y] >= 1) 
                    {
                        queueDFS.Enqueue(new Vector3Int(newPos.x, newPos.y, queuePos.z));
                        visited.Add(newPos);
                        queued = true;
                    }
                }
                randomPool.RemoveAt(randomIndex);
            }
            if (!queued && queuePos.z >= minStep) // if the DFS reaches dead end, go here, unless it is less than the min step
            {
                break;
            }

        }
        
        return new Vector2Int(queuePos.x, queuePos.y)
    }

    private List<int> SetupPooledDirections()
    {
        return new List<int>
        {
            0, 1, 2, 3
        };
    }

    
}
