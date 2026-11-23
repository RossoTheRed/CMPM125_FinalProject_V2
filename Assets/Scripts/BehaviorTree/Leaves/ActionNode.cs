using System;

// Note: Named action node due to clashing libraries
public class ActionNode : LeafNode
{
    // Note: If you want to not pass in actions, it is fine, just make sure to override Update

    private Func<Status> _action;
    public ActionNode(Func<Status> action = null) 
    { 
        _action = action; 
        m_eTickType = TickType.BH_LOGIC; // default is logic, feel free to change
    }
    
    protected override Status Update() => _action();
    protected override Status FixedUpdate() => GetStatus(); 


}
