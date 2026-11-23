class Sequence : Composite
{
    protected int m_CurrentChild;
    public override void OnInitialize() => m_CurrentChild = 0;

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

    // If update or sequence gets told its still running, then the fixed update will run through when available
    // Fixedupdate when called on fixed frame, basically goes back and forth between update and fixed update
    // FixedUpdate takes backseat, but when it is called, does an iteration of the current child, then in its while loop, while
    // every new iteration in the update loop returns and sets some status of Running, it continues
    // its own while loop, but will stop if its fixed tick gives either invalid or valid
    // Because it only runs on fixed frame, and only needs to check for success or failure
    // it not only checks if it is running but as internal counter ot make sure its not constantly checking
   // that way it can return that its still running but not be in a loop
    
    // NOTE: Any process like moveTo that takes multiple frames, must return running
    protected override Status FixedUpdate()
    {
        if (GetStatus() != Status.BH_RUNNING) return GetStatus();
        
        while (m_CurrentChild < m_Children.Count)
        {
            Status status = m_Children[m_CurrentChild].FixedTick();
            if (status == Status.BH_SUCCESS)// run the next child
            {
                m_CurrentChild++;
            }
            else
            {
                return status;
            }

            if (m_CurrentChild >= m_Children.Count)
            {
                return Status.BH_SUCCESS; 
            }

            Status tickStatus = Tick();
            if (tickStatus == Status.BH_RUNNING) continue;
            else if (tickStatus == Status.BH_SUCCESS) return Status.BH_SUCCESS;
            else return tickStatus;
            
        }
        return Status.BH_INVALID;
    }
}
