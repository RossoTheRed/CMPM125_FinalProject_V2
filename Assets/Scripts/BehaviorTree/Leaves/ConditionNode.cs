using System;
public class ConditionNode : LeafNode
{
    // Note: If you want to not pass in condition, it is fine, just make sure to override Update
    protected Func<bool> _condition;
    public ConditionNode(Func<bool> condition = null) 
    { 
        _condition = condition; 
        m_eTickType = TickType.BH_LOGIC; // default is logic, feel free to change

    }
    
    protected override Status Update() => _condition() ? Status.BH_SUCCESS : Status.BH_FAILURE;
    protected override Status FixedUpdate() => GetStatus(); 

}