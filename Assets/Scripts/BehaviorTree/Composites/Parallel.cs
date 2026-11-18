public class Parallel : Composite
{
    public enum Policy
    {
        REQUIREONE,
        REQUIREALL
    };
    protected Policy m_eSuccessPolicy;
    protected Policy m_eFailurePolicy;
    public Parallel (Policy success, Policy failure)
    {
        m_eSuccessPolicy = success;
        m_eFailurePolicy = failure;
    }

    public override void OnInitialize()
    {
        foreach (var child in m_Children)
        {
            if (child.GetStatus() == Status.BH_SUCCESS || child.GetStatus() == Status.BH_FAILURE)
            {
                child.Reset();
            }
        }
        base.OnInitialize();
    }

    protected override Status Update()
    {
        int iSuccessCount = 0, iFailureCount = 0;
        foreach (var child in m_Children)
        {
            if (child.IsTerminated()) child.Tick();
            if (child.GetStatus() == Status.BH_SUCCESS)
            {
                ++iSuccessCount;
                if (m_eSuccessPolicy == Policy.REQUIREONE) return Status.BH_SUCCESS;
            }

            if (child.GetStatus() == Status.BH_FAILURE)
            {
                ++iFailureCount;
                if (m_eFailurePolicy == Policy.REQUIREONE) return Status.BH_FAILURE;
            }

        }
        if (m_eFailurePolicy == Policy.REQUIREALL && iFailureCount == m_Children.Count) return Status.BH_FAILURE;
        if (m_eSuccessPolicy == Policy.REQUIREALL && iSuccessCount == m_Children.Count) return Status.BH_SUCCESS;
        return Status.BH_RUNNING;
    }

    protected override void OnTerminate(Status status)
    {
        foreach (var child in m_Children)
        {
            if (child.IsRunning()) child.Abort();
        }
    }
}
