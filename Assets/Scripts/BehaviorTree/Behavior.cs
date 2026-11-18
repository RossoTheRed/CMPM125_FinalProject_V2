public enum Status
{
    BH_INVALID,
    BH_SUCCESS,
    BH_FAILURE,
    BH_RUNNING,
    BH_ABORTED,
};
    
public class Behavior
{
    // maybe make this read only
    private Status m_eStatus;

    public Behavior()
    {
        m_eStatus = Status.BH_INVALID;
    }

    ~Behavior() {}

    public virtual void OnInitialize() {}
    // maybe make this an abstract class, or make a dedicated leaf class
    public virtual Status Update() {return m_eStatus;}
    protected virtual void FixedUpdate() {}
    public virtual void OnTerminate(Status status) {}

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

}
