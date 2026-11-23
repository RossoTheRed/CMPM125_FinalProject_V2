public class ForceSuccess : Decorator
{
    public ForceSuccess(Behavior child) : base(child) {}
    protected override Status Update()
    {
        m_pChild.Tick();
        if (m_pChild.GetStatus() == Status.BH_RUNNING) return Status.BH_RUNNING;
        // NOTE: not confident on this addition I added - chris
        if (m_pChild.GetStatus() == Status.BH_INVALID) return Status.BH_INVALID;

        return Status.BH_SUCCESS;

        // The book intentionally leaves this here as a defensive fallback.
        #pragma warning disable CS0162 
        return Status.BH_INVALID; 
        #pragma warning restore CS0162
        
    }

    protected override Status FixedUpdate()
    {
        return m_pChild.FixedTick();
    }   
}
