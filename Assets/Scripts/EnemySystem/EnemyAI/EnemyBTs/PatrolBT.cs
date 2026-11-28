using UnityEngine;
using EnemyBT.Conditions;

namespace EnemyBT.Tree
{
    
public class PatrolBT : EnemyBaseBT
{

    public PatrolBT(EnemyBlackboard enemyBlackboard) : base(enemyBlackboard) {}
    protected override BehaviorTreeBuilder BuildBehaviorTree(EnemyBlackboard eBoard)
    {
        Debug.Log("Built");
        BehaviorTreeBuilder bt = new BehaviorTreeBuilder()
        .Selector()
            .Sequence() // should we chase
                .Condition(new LineOfSight(eBoard))
                .Action(new NavToLocation(eBoard))
            .End()
            .Sequence() // pure wander random pathfind
                .Condition(new PredictedRoomExist(eBoard))
                .Action(new NavToPredictedRoom(eBoard))
            .End()
            .Sequence()
                

            .End()
            .Sequence()
                .Action(new HaltAcceleration(eBoard));

        return bt;
    }
}

}