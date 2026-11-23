using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// template class
public abstract class BehaviorTree
{
    protected Behavior m_pRoot;
    //protected BehaviorTreeBuilder behaviorTreeBuilder;
    public BehaviorTree()
    {
        
    }

    //protected abstract BehaviorTreeBuilder BuildBehaviorTree();
        //BehaviorTreeBuilder bt = new BehaviorTreeBuilder()
        // put your methods here
        //.End();

        //behaviorTreeBuilder = bt;
        //m_pRoot = bt.Build();

    public Behavior GetRootNode()
    {
        return m_pRoot;
    }

    public void OnInitialize()
    {
        if (m_pRoot == null) return;
        
        m_pRoot.OnInitialize();
    }
    public void Tick()
    {
        if (m_pRoot == null /*|| m_pRoot.IsTerminated()*/) return;
        Debug.Log("Ticking");
        m_pRoot.Tick();
    }

    public void FixedTick()
    {
        if (m_pRoot == null) return;
        Debug.Log("FixedTicking");
        m_pRoot.FixedTick();
    }

}
