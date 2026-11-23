using System;
using UnityEngine;

namespace EnemyBT.Conditions 
{
public class PredictedRoomExist : ConditionNode
{
    private EnemyBlackboard enemyBlackboard;
    public PredictedRoomExist(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
        m_eTickType = TickType.BH_LOGIC;
    }

    sealed protected override Status Update()
    {
        Debug.Log("Checking");
        bool condition = enemyBlackboard._isValidPrediction;
        Debug.Log($"Condition {condition}");

        return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
    }
}

}
