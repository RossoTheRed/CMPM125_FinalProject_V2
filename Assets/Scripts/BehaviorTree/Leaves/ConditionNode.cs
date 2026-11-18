using System;
public class ConditionNode : Leaf
{
    private Func<bool> _condition;
    public ConditionNode(Func<bool> condition) { _condition = condition; }
    
    protected override Status Update() => _condition() ? Status.BH_SUCCESS : Status.BH_FAILURE;
}