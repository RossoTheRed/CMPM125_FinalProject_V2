using System.Collections.Generic;

public abstract class Composite : Behavior
{
    protected List<Behavior> m_Children = new List<Behavior>();
    protected bool physicsFrame = false;
    public Composite() { m_eTickType = TickType.BH_BOTH; }
    public override void Abort()
    {
        foreach (var child in m_Children)
        {
            if (child.GetStatus() == Status.BH_RUNNING)
            {
                child.Abort();
            }
        }
        base.Abort();
    }
    /*public override Status Tick()
    {
        Status currentStatus = Status.BH_SUCCESS;
        if (GetStatus() != Status.BH_RUNNING) return Status.BH_RUNNING;
        foreach (var child in m_Children)
        {
            if (child.GetStatus() == Status.BH_RUNNING)
            {
                currentStatus = child.Tick();
            }
            if (currentStatus == Status.BH_RUNNING) return Status.BH_RUNNING;
            else if (currentStatus == Status.BH_FAILURE) return Status.BH_FAILURE;
        }

        return Status.
    }*/

    /*public override void FixedTick()
    {
        Status currStatus = Status.BH_INVALID;
        if (GetStatus() == Status.BH_RUNNING)
        {
            currStatus = FixedUpdate();
        }
        if (currStatus == Status.BH_SUCCESS) Tick();
    }*/

    public void AddChild(Behavior child) => m_Children.Add(child);
    public void RemoveChild(Behavior child) => m_Children.Remove(child);
    public void ClearChildren() => m_Children.Clear();

    //protected override Status Update() {return Status.BH_FAILURE;}
}
