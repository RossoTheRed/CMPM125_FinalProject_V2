using UnityEngine;


namespace EnemyBT.Tree
{
    

public abstract class EnemyBaseBT : BehaviorTree
{
    public EnemyBaseBT(EnemyBlackboard enemyBlackboard)
    {
        m_pRoot = BuildBehaviorTree(enemyBlackboard).Build();
    }

    protected abstract BehaviorTreeBuilder BuildBehaviorTree(EnemyBlackboard enemyBlackboard);
}

}