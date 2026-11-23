using UnityEngine;
using EnemyBT.Conditions;

namespace EnemyBT.Tree
{
    
public class ChaseBT : EnemyBaseBT
{

    public ChaseBT(EnemyBlackboard enemyBlackboard) : base(enemyBlackboard) {}
    protected override BehaviorTreeBuilder BuildBehaviorTree(EnemyBlackboard eBoard)
    {
        Debug.Log("Built");
        BehaviorTreeBuilder bt = new BehaviorTreeBuilder()
        .Selector()
            .Sequence() // close enough
                .Condition(new LineOfSight(eBoard))
                .Action(new NavToLocation(eBoard))
            .End()
            .Sequence() // dart to predicted room
            .End()
            .Sequence() // break off & predict
                .Selector() // Stall & Hide in adjacent room
                    
                

            .End()
            .Sequence()
                .Action(new HaltAcceleration(eBoard));

        return bt;
    }
}

}