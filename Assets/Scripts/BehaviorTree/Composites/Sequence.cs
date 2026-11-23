class Sequence : Composite
{
    protected int m_CurrentChild;
    public override void OnInitialize() => m_CurrentChild = 0;

    protected override Status Update()
    {
        while (m_CurrentChild < m_Children.Count)
        {
            // if physics only, wait for it to finish
            if (m_Children[m_CurrentChild].GetTickType() == TickType.BH_PHYSICS) return Status.BH_RUNNING;
            Status status = m_Children[m_CurrentChild].Tick();
            if (status != Status.BH_SUCCESS) return status;
            if (++m_CurrentChild >= m_Children.Count) return Status.BH_SUCCESS; 
        }

        // The book intentionally leaves this here as a defensive fallback.
        #pragma warning disable CS0162 
        return Status.BH_INVALID; 
        #pragma warning restore CS0162
    }

    // If update or sequence gets told its still running, then the fixed update will run through when available
    // Fixedupdate when called on fixed frame, basically goes back and forth between update and fixed update
    // FixedUpdate takes backseat, but when it is called, does an iteration of the current child, then in its while loop, while
    // every new iteration in the update loop returns and sets some status of Running, it continues
    // its own while loop, but will stop if its fixed tick gives either invalid or valid
    // Because it only runs on fixed frame, and only needs to check for success or failure
    // it not only checks if it is running but as internal counter ot make sure its not constantly checking
   // that way it can return that its still running but not be in a loop
    

    protected override Status FixedUpdate()
    {
        int pastCount = 0; 
        // current child is how far we got, we just need to check to make it didnt fail (shouldnt be the case)
        // will run up to where we are at, if not failure, run the physics only and both
        while (pastCount < m_CurrentChild)
        {
            // failed, probably don't go on, even though it shouldn't due to pastCount < current child then
            if (m_Children[pastCount].GetStatus() == Status.BH_FAILURE) return Status.BH_FAILURE;
            Status status = Status.BH_SUCCESS; // standard if its normal tick
            if (m_Children[pastCount].GetTickType() == TickType.BH_BOTH || m_Children[pastCount].GetTickType() == TickType.BH_PHYSICS) // runs both
            {
                status = m_Children[pastCount].FixedTick(); // should always return true in this case, because it is behind
            }
            if (status == Status.BH_FAILURE) return Status.BH_FAILURE;
            pastCount++;
        }

        // check if the current child failed, or if it was just running
        if (m_Children[pastCount].GetStatus() == Status.BH_FAILURE) return Status.BH_FAILURE;
        while (m_CurrentChild < m_Children.Count)
        {
            Behavior currChild = m_Children[pastCount];
            if (currChild.GetTickType() == TickType.BH_LOGIC)
            {
                Status logicStatus = currChild.Tick();
                if (logicStatus != Status.BH_SUCCESS) return logicStatus;
                // passed
            }
            else // physics or both (THIS IS CATCHING UP)
            {
                Status status = Status.BH_INVALID;
                if (currChild.GetTickType() == TickType.BH_BOTH)
                {
                    status = currChild.Tick();
                    if (status == Status.BH_FAILURE) return Status.BH_FAILURE;
                    // success it doesn't matter, running means its waiting for a physics loop,
                    // which we are about to do - chris
                }
                status = currChild.FixedTick();
                if (status == Status.BH_FAILURE) return status;
                if (status == Status.BH_RUNNING) return status;
            }
            
            m_CurrentChild++;
            if (++m_CurrentChild >= m_Children.Count) return Status.BH_SUCCESS; 
        }
        
        #pragma warning disable CS0162 
        return Status.BH_SUCCESS; 
        #pragma warning restore CS0162
    }
}
