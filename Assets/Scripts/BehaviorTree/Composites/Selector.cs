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
}

