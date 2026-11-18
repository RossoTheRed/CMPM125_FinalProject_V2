using System.Collections.Generic;

public class Composite : Behavior
{
    protected List<Behavior> m_Children = new List<Behavior>();

    public void AddChild(Behavior child) => m_Children.Add(child);
    public void RemoveChild(Behavior child) => m_Children.Remove(child);
    public void ClearChildren() => m_Children.Clear();
    
}
