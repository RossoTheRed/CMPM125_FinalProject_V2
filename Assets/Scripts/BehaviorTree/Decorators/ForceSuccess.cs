public class ForceSuccess : Decorator
{
    public ForceSuccess(Behavior child) : base(child) {}
    protected override Status Update()
    {
        while (true)
        {
            m_pChild.Tick();
            if (m_pChild.GetStatus() == Status.BH_RUNNING) return Status.BH_RUNNING;
            // NOTE: not confident on this addition I added - chris
            if (m_pChild.GetStatus() == Status.BH_INVALID) return Status.BH_INVALID;

            return Status.BH_SUCCESS;
        }

        // The book intentionally leaves this here as a defensive fallback.
        #pragma warning disable CS0162 
        return Status.BH_INVALID; 
        #pragma warning restore CS0162
        /*
        m_pChild.Tick();
        Status status = m_pChild.GetStatus();
        if (status == Status.BH_RUNNING) return Status.BH_RUNNING;
        if (status == Status.BH_INVALID) return Status.BH_INVALID;
        
        if (status == Status.BH_FAILURE) return Status.BH_SUCCESS;
        if (status == Status.BH_SUCCESS) return Status.BH_SUCCESS;
        else return status;
        */
    }
}
