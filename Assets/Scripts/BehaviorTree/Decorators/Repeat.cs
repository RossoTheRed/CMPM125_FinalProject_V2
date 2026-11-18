public class Repeat : Decorator
{
    private int m_iLimit;
    private int m_iCounter;
    public Repeat(int limit, Behavior child) : base(child)
    {
        m_iLimit = limit;
        m_iCounter = 0;
    }
    protected override Status Update()
    {
        while (true)
        {
            m_pChild.Tick();
            if (m_pChild.GetStatus() == Status.BH_RUNNING) return Status.BH_RUNNING;
            if (m_pChild.GetStatus() == Status.BH_INVALID) return Status.BH_INVALID;

            if (m_pChild.GetStatus() == Status.BH_FAILURE) return Status.BH_FAILURE;
            if (++m_iCounter >= m_iLimit) return Status.BH_SUCCESS;
        }

        // The book intentionally leaves this here as a defensive fallback.
        #pragma warning disable CS0162 
        return Status.BH_INVALID; 
        #pragma warning restore CS0162
    }
}
