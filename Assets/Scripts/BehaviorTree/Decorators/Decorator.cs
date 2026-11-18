using Unity.Mathematics;

public abstract class Decorator : Behavior
{
    protected Behavior m_pChild;
    public Decorator(Behavior child) => m_pChild = child;
}