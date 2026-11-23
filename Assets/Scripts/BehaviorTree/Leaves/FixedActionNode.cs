using System;

public class FixedActionNode : LeafNode
{
    // Note: If you want to not pass in actions, it is fine, just make sure to override Update

    private Func<Status> _action;
    private Func<Status> _fixedAction;
    public FixedActionNode(Func<Status> fixedAction = null, Func<Status> action = null) 
    { 
        _fixedAction = fixedAction;
        _action = action; 
    }

    protected override Status Update()
    {
        if (_action == null) return Status.BH_SUCCESS;
        else return _action();
    }
    protected override Status FixedUpdate()
    {
        if (_fixedAction == null) return Status.BH_INVALID;
        else
        {
            return _fixedAction();   
        }
    }
}
