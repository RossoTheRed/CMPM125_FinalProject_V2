using UnityEngine;
using EnemyBT.Conditions;

namespace EnemyBT.Tree
{
    

public class CalmBT : EnemyBaseBT
{
    public CalmBT(EnemyBlackboard enemyBlackboard) : base(enemyBlackboard) {}

    protected override BehaviorTreeBuilder BuildBehaviorTree(EnemyBlackboard eBoard)
    {
        Debug.Log("Built");
        BehaviorTreeBuilder bt = new BehaviorTreeBuilder()
        .Selector()
            .Sequence() // should we chase
                .Action(DFS())

        return bt;
    }
}

}