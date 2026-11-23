using UnityEngine;
using EnemyBT.Conditions;

namespace EnemyBT.Tree
{
    
public class PatrolBT : EnemyBaseBT
{

    public PatrolBT(EnemyBlackboard enemyBlackboard) : base(enemyBlackboard) {}
    protected override BehaviorTreeBuilder BuildBehaviorTree(EnemyBlackboard enemyBlackboard)
    {
        Debug.Log("Built");
        BehaviorTreeBuilder bt = new BehaviorTreeBuilder()
        .Sequence()
            .Condition(new LineOfSight(enemyBlackboard))
            .Action(new NavToLocation(enemyBlackboard));

        return bt;
    }
}

}