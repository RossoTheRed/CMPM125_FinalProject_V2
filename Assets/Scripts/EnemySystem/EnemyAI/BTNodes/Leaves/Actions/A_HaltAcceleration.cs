using EnemyBT.Actions;
using UnityEngine;

public class HaltAcceleration : ActionNode
{
    private EnemyBlackboard enemyBlackboard;
    public HaltAcceleration(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
        m_eTickType = TickType.BH_BOTH;
    }

    public sealed override void OnInitialize()
    {
        enemyBlackboard.enemyFSM.DisableNavAgent();
    }
    
    sealed protected override Status Update() 
    {
        // because there is a fixed update, it should go and commit those actions
        return Status.BH_SUCCESS;
    }

    sealed protected override Status FixedUpdate()
    {
        Debug.Log("halting");
        GeneralActions.BasicHalt(enemyBlackboard.enemyFSM.sourceRB, enemyBlackboard.enemyFSM.maxSpeed);
        return Status.BH_SUCCESS;
    }
}
