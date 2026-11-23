using UnityEngine;
class Selector : Composite
{
    protected int m_CurrentChild;
    public override void OnInitialize() => m_CurrentChild = 0;

    sealed protected override Status Update()
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
    sealed protected override Status FixedUpdate()
    {
        Debug.Log($"Fixed Update Selector");
        int pastCount = 0; 
        // current child is how far we got, we just need to check to make it didnt fail (shouldnt be the case)
        // will run up to where we are at, if not successs, run the physics only and both
        Debug.Log($"Past Count {pastCount}");

        while (pastCount < m_CurrentChild)
        {
            // success, probably don't go on, even though it shouldn't due to pastCount < current child then
            if (m_Children[pastCount].GetStatus() == Status.BH_SUCCESS) return Status.BH_SUCCESS;
            Status status = Status.BH_FAILURE; // standard if its normal tick
            if (m_Children[pastCount].GetTickType() == TickType.BH_BOTH) // runs both
            {
                status = m_Children[pastCount].FixedTick(); // should always return true in this case, because it is behind
                if (status == Status.BH_SUCCESS) return Status.BH_SUCCESS;
            }

            pastCount++;
            Debug.Log($"Past Count {pastCount}");
        }
        if (pastCount >= m_Children.Count) return GetStatus();

        Debug.Log($"Checking if child failed");
        // check if the current child success, or if it was just running
        if (m_Children[pastCount].GetStatus() == Status.BH_SUCCESS && m_Children[pastCount].GetTickType() == TickType.BH_LOGIC) return Status.BH_SUCCESS;
        while (m_CurrentChild < m_Children.Count)
        {
            Behavior currChild = m_Children[m_CurrentChild];
            if (currChild.GetTickType() == TickType.BH_LOGIC)
            {
                Status logicStatus = currChild.Tick();
                Debug.Log($"Checking logic Status {logicStatus}");
                if (logicStatus != Status.BH_FAILURE) return logicStatus;
                // passed
            }
            else // physics or both (THIS IS CATCHING UP)
            {
                Status physicsStatus = Status.BH_INVALID;
                Status logicStatus = Status.BH_INVALID;
                if (currChild.GetTickType() == TickType.BH_BOTH)
                {
                    logicStatus = currChild.Tick();
                    Debug.Log($"Checking both Status {logicStatus}");
                    // failure it doesn't matter, running means its waiting for a physics loop,
                    // which we are about to do - chris
                }
                
                physicsStatus = currChild.FixedTick();
                Debug.Log($"Checking both or fixed Status {physicsStatus}");
                if (logicStatus == Status.BH_INVALID) // situation for physics only
                {
                    if (physicsStatus == Status.BH_SUCCESS) return physicsStatus;
                    if (physicsStatus == Status.BH_RUNNING) return physicsStatus;
                }
                else // situation for both, treats them both needing to be true in order to leave?
                {
                    if (logicStatus != Status.BH_FAILURE)
                    {
                        if (physicsStatus == Status.BH_RUNNING) return physicsStatus;
                        if (physicsStatus == Status.BH_SUCCESS) return physicsStatus;
                    }
                }
            }
            Debug.Log($"next child");
            if (++m_CurrentChild >= m_Children.Count) return Status.BH_FAILURE; 
        }
        
        #pragma warning disable CS0162 
        return Status.BH_FAILURE; 
        #pragma warning restore CS0162
    }
}

