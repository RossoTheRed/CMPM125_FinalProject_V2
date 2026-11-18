using System;

// Note: Named action node due to clashing libraries
public class ActionNode : Leaf
{
    private Func<Status> _action;
    public ActionNode(Func<Status> action) { _action = action; }
    
    protected override Status Update() => _action();
}
