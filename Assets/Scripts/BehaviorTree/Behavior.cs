public enum Status
{
    BH_INVALID,
    BH_SUCCESS,
    BH_FAILURE,
    BH_RUNNING,
    BH_ABORTED,
};

public enum TickType
{
    BH_LOGIC,
    BH_FIXED,
    BH_BOTH
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

    // currently this is public, might need to be protected only
    public virtual void OnInitialize() {}
    // maybe make this an abstract class, or make a dedicated leaf class
    protected abstract Status Update();
    protected virtual Status FixedUpdate() { return Status.BH_SUCCESS;}
    protected virtual void OnTerminate(Status status) {}
    public virtual void Abort()
    {
        OnTerminate(Status.BH_ABORTED);
        m_eStatus = Status.BH_INVALID;
    }

    public virtual Status Tick()
    {
        if (m_eStatus != Status.BH_RUNNING) OnInitialize();
        m_eStatus = Update();
        if (m_eStatus != Status.BH_RUNNING) OnTerminate(m_eStatus);
        return m_eStatus;
    }

    // NOTE: Major fix idea, have it to where every time fixedTick is called, tick, if it sees that it is true
    // that this next frame it will run all the fixed frames/updates

    // NOTE: Because of this conditions which check during fixed should always
    // return false to prevent mass faults, but most conditions are not fixed

    // Note: because of this, actions which run during fixed, run always true
    // as to allow for later actions not locked in fixed to keep going 

    public virtual Status FixedTick()
    {
        if (m_eStatus == Status.BH_RUNNING)
        {
            return FixedUpdate();
        }
        return Status.BH_INVALID;
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
