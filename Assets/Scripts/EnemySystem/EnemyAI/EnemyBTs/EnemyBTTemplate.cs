using UnityEngine;

public class EnemyBTTemplate
{
    protected virtual BehaviorTreeBuilder BuiltBehaviorTree()
    {
        BehaviorTreeBuilder bt = new BehaviorTreeBuilder()
        // put your methods here
        .End();

        return bt;
    }
    public BehaviorTreeBuilder GetBT()
    {
        return BuiltBehaviorTree();
    }
}
