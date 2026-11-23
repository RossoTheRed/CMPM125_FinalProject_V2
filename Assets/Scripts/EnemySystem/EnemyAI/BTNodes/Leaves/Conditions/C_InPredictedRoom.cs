using System;
using UnityEngine;

namespace EnemyBT.Conditions 
{
public class InPredictedRoom : ConditionNode
{
    private EnemyBlackboard enemyBlackboard;
    public InPredictedRoom(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
        m_eTickType = TickType.BH_LOGIC;
    }

    sealed protected override Status Update()
    {
        Debug.Log("Checking");
        Vector3 locationPosition = enemyBlackboard.GetPredictedRoomPosition();
        Vector3 enemyPosition = enemyBlackboard.enemyFSM.transform.position;
        float distance = Vector3.Distance(locationPosition, enemyPosition);
        bool condition = distance < enemyBlackboard.GetRoomSize();
        Debug.Log($"Condition {condition}");

        return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
    }
}

}
