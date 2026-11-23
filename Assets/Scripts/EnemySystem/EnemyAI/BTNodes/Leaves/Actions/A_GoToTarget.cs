using EnemyBT.Actions;
using UnityEngine;

public class GoToTarget : ActionNode
{
    private EnemyBlackboard enemyBlackboard;
    public GoToTarget(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
    }
    
    protected override Status Update() 
    {
        // because there is a fixed update, it should go and commit those actions
        return Status.BH_RUNNING;
    }

    protected override Status FixedUpdate()
    {
        Debug.Log("moving");
        GeneralActions.RotateTowards(enemyBlackboard.enemyFSM.sourceRB, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.rotateSpeed);
        GeneralActions.BasicChase(enemyBlackboard.enemyFSM.sourceRB, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.maxSpeed);
        return Status.BH_RUNNING;
    }
}
