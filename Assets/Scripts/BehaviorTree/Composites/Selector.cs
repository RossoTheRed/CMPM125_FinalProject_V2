class Selector : Composite
{
    protected int m_CurrentChild;
    public override void OnInitialize() => m_CurrentChild = 0;

    protected override Status Update()
    {
        while (m_CurrentChild < m_Children.Count)
        {
            Status status = m_Children[m_CurrentChild].Tick();
            if (status != Status.BH_FAILURE) return status;
            if (++m_CurrentChild >= m_Children.Count) return Status.BH_FAILURE; 
        }

        // The book intentionally leaves this here as a defensive fallback.
        #pragma warning disable CS0162 
        return Status.BH_INVALID; 
        #pragma warning restore CS0162
    }

    // NOTE: Any process like moveTo that takes multiple frames, must return running
    protected override Status FixedUpdate()
    {
        if (GetStatus() != Status.BH_RUNNING) return GetStatus();
        
        while (m_CurrentChild < m_Children.Count)
        {
            Status status = m_Children[m_CurrentChild].FixedTick();
            if (status == Status.BH_FAILURE)// run the next child
            {
                m_CurrentChild++;
            }
            else
            {
                return status;
            }

            if (m_CurrentChild >= m_Children.Count)
            {
                return Status.BH_FAILURE; 
            }

            Status tickStatus = Tick();
            if (tickStatus == Status.BH_RUNNING) continue;
            else if (tickStatus == Status.BH_SUCCESS || tickStatus == Status.BH_INVALID) return tickStatus;
            
        }
        return Status.BH_INVALID;
    }
}

