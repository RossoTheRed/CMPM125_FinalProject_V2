using System;
using UnityEngine;

namespace EnemyBT.Conditions 
{
public class TargetLocationKnown : ConditionNode
{
    private EnemyBlackboard enemyBlackboard;
    public TargetLocationKnown(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
        m_eTickType = TickType.BH_LOGIC;
    }

    sealed protected override Status Update()
    {
        Debug.Log("Checking");
        bool condition = enemyBlackboard._isLocationKnown;
        Debug.Log($"Condition {condition}");

        return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
    }
}

}
