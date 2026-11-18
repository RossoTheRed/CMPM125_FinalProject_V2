public enum Status
{
    BH_INVALID,
    BH_SUCCESS,
    BH_FAILURE,
    BH_RUNNING,
    BH_ABORTED,
};
    
public abstract class Behavior
{
    // maybe make this read only
    private Status m_eStatus;

    public Behavior()
    {
        m_eStatus = Status.BH_INVALID;
    }

    ~Behavior() {}

    protected virtual void OnInitialize() {}
    // maybe make this an abstract class, or make a dedicated leaf class
    protected abstract Status Update();
    protected virtual void FixedUpdate() {}
    protected virtual void OnTerminate(Status status) {}
    public virtual void Abort()
    {
        OnTerminate(Status.BH_ABORTED);
        m_eStatus = Status.BH_INVALID;
    }

    public Status Tick()
    {
        if (m_eStatus != Status.BH_RUNNING) OnInitialize();
        m_eStatus = Update();
        if (m_eStatus != Status.BH_RUNNING) OnTerminate(m_eStatus);
        return m_eStatus;
    }

    public void FixedTick()
    {
        if (m_eStatus == Status.BH_RUNNING)
        {
            FixedUpdate();
        }
    }

    public void Reset()
    {
        m_eStatus = Status.BH_INVALID;
        OnInitialize();
    }

    public Status GetStatus()
    {
        return m_eStatus;
    }

    public bool IsTerminated()
    {
        return m_eStatus == Status.BH_SUCCESS || m_eStatus == Status.BH_FAILURE;
    }

    public bool IsRunning()
    {
        return m_eStatus == Status.BH_RUNNING;
    }

}
