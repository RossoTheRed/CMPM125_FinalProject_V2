class Sequence : Composite
{
    protected int m_CurrentChild;
    protected override void OnInitialize() => m_CurrentChild = 0;

    protected override Status Update()
    {
        while (m_CurrentChild < m_Children.Count)
        {
            Status status = m_Children[m_CurrentChild].Tick();
            if (status != Status.BH_SUCCESS) return status;
            if (++m_CurrentChild >= m_Children.Count) return Status.BH_SUCCESS; 
        }

        // The book intentionally leaves this here as a defensive fallback.
        #pragma warning disable CS0162 
        return Status.BH_INVALID; 
        #pragma warning restore CS0162
    }
}
