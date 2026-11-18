using System.Collections.Generic;

public abstract class Composite : Behavior
{
    protected List<Behavior> m_Children = new List<Behavior>();
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

    public void AddChild(Behavior child) => m_Children.Add(child);
    public void RemoveChild(Behavior child) => m_Children.Remove(child);
    public void ClearChildren() => m_Children.Clear();

    //protected override Status Update() {return Status.BH_FAILURE;}
}
