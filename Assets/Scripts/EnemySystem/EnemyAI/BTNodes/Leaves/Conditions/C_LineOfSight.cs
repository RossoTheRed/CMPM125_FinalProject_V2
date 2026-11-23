using System;
using UnityEngine;

namespace EnemyBT.Conditions 
{
public class LineOfSight : ConditionNode
{
    private EnemyBlackboard enemyBlackboard;
    public LineOfSight(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
        m_eTickType = TickType.BH_LOGIC;
    }

    sealed protected override Status Update()
    {
        Debug.Log("Checking");
        bool condition = GeneralConditions.HasLineOfSight(enemyBlackboard.enemyFSM.transform, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.sightDist, enemyBlackboard.enemyFSM.targetMask, enemyBlackboard.enemyFSM.collisionSightMask);
        Debug.Log($"Condition {condition}");
        return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
    }
}

}
