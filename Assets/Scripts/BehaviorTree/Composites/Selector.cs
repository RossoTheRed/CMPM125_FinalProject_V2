class Selector : Composite
{
    protected int m_CurrentChild;
    public override void OnInitialize() => m_CurrentChild = 0;

    protected override Status Update()
    {
        while (m_CurrentChild < m_Children.Count)
        {
            // probably should just have this tick anyway, and have it return running
            if (m_Children[m_CurrentChild].GetTickType() == TickType.BH_PHYSICS) return Status.BH_RUNNING;
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
        int pastCount = 0; 
        // current child is how far we got, we just need to check to make it didnt fail (shouldnt be the case)
        // will run up to where we are at, if not successs, run the physics only and both
        while (pastCount < m_CurrentChild)
        {
            // success, probably don't go on, even though it shouldn't due to pastCount < current child then
            if (m_Children[pastCount].GetStatus() == Status.BH_SUCCESS) return Status.BH_SUCCESS;
            Status status = Status.BH_FAILURE; // standard if its normal tick
            if (m_Children[pastCount].GetTickType() == TickType.BH_BOTH || m_Children[pastCount].GetTickType() == TickType.BH_PHYSICS) // runs both
            {
                status = m_Children[pastCount].FixedTick(); // should always return true in this case, because it is behind
            }
            if (status == Status.BH_SUCCESS) return Status.BH_SUCCESS;
            pastCount++;
        }

        // check if the current child success, or if it was just running
        if (m_Children[pastCount].GetStatus() == Status.BH_SUCCESS) return Status.BH_SUCCESS;
        while (m_CurrentChild < m_Children.Count)
        {
            Behavior currChild = m_Children[pastCount];
            if (currChild.GetTickType() == TickType.BH_LOGIC)
            {
                Status logicStatus = currChild.Tick();
                if (logicStatus != Status.BH_FAILURE) return logicStatus;
                // passed
            }
            else // physics or both (THIS IS CATCHING UP)
            {
                Status status = Status.BH_INVALID;
                if (currChild.GetTickType() == TickType.BH_BOTH)
                {
                    status = currChild.Tick();
                    if (status == Status.BH_SUCCESS) return Status.BH_SUCCESS;
                    // failure it doesn't matter, running means its waiting for a physics loop,
                    // which we are about to do - chris
                }
                status = currChild.FixedTick();
                if (status == Status.BH_SUCCESS) return status;
                if (status == Status.BH_RUNNING) return status;
            }
            
            m_CurrentChild++;
            if (++m_CurrentChild >= m_Children.Count) return Status.BH_FAILURE; 
        }
        
        #pragma warning disable CS0162 
        return Status.BH_FAILURE; 
        #pragma warning restore CS0162
    }
}

