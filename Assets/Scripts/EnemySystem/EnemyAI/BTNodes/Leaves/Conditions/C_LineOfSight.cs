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
    }

    protected override Status Update()
    {
        Debug.Log("Checking");
        bool condition = GeneralConditions.HasLineOfSight(enemyBlackboard.enemyFSM.transform, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.sightDist, enemyBlackboard.enemyFSM.targetMask, enemyBlackboard.enemyFSM.collisionSightMask);
        lastCondition = condition;
        return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
    }
}

}
