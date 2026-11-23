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
        m_eTickType = TickType.BH_BOTH;
    }

    protected override Status Update()
    {
        if (_action == null) return Status.BH_SUCCESS; // returns success default to let all actions to be played through
        else return _action();
    }
    protected override Status FixedUpdate()
    {
        if (_fixedAction == null) return Status.BH_SUCCESS; // no fixed action exists, why are you using this?
        else
        {
            return _fixedAction(); // either running, failure, or success
        }
    }
}
