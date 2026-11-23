using System;
public class ConditionNode : LeafNode
{
    // Note: If you want to not pass in condition, it is fine, just make sure to override Update
    protected Func<bool> _condition;
    protected bool lastCondition = false; // last time the condition ran, this is the state
    public ConditionNode(Func<bool> condition = null) { _condition = condition; }
    
    protected override Status Update() => _condition() ? Status.BH_SUCCESS : Status.BH_FAILURE;
    protected override Status FixedUpdate() => lastCondition ? Status.BH_SUCCESS : Status.BH_FAILURE;

}