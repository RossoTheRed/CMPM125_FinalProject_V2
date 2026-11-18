public class ForceSuccess : Decorator
{
    public ForceSuccess(Behavior child) : base(child) {}
    protected override Status Update()
    {
        m_pChild.Tick();
        Status status = m_pChild.GetStatus();
        if (status == Status.BH_RUNNING) return Status.BH_RUNNING;
        if (status == Status.BH_INVALID) return Status.BH_INVALID;
        
        if (status == Status.BH_FAILURE) return Status.BH_SUCCESS;
        if (status == Status.BH_SUCCESS) return Status.BH_SUCCESS;
        else return status;
    }
}
