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
        m_eTickType = TickType.BH_BOTH;
    }

    sealed protected override Status Update()
    {
        bool condition = GeneralConditions.HasLineOfSight(enemyBlackboard.enemyFSM.transform, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.sightDist, enemyBlackboard.enemyFSM.targetMask, enemyBlackboard.enemyFSM.collisionSightMask);
        return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
        //return GetStatus();
    }

    protected sealed override Status FixedUpdate()
    {
        return GetStatus();
        //Debug.Log("Checking");
        //bool condition = GeneralConditions.HasLineOfSight(enemyBlackboard.enemyFSM.transform, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.sightDist, enemyBlackboard.enemyFSM.targetMask, enemyBlackboard.enemyFSM.collisionSightMask);
        //return condition ? Status.BH_SUCCESS : Status.BH_FAILURE;
    }
}

}
