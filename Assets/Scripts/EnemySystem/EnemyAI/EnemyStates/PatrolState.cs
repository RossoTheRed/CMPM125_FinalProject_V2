using UnityEngine;

namespace EnemyStates {

public class PatrolState : StateComponent<PlayerStateMachine>
{
    private Behavior behaviorTreeRoot = null;
    public PatrolState(Behavior root, PlayerStateMachine machine) : base(machine)
    {
        Initialize(root);
    }

    // Initialize 
    // Will start up the enemy behavior tree if one exists
    public void Initialize(Behavior root)
    {
        behaviorTreeRoot = root;
        if (behaviorTreeRoot != null) behaviorTreeRoot.OnInitialize();

    }
    public override void Enter(StateComponent previousState)
    {
        if (behaviorTreeRoot != null) behaviorTreeRoot.OnInitialize();
    }
    public override void Exit(StateComponent nextState) {}
    public override void Update(float dt)
    {
        if (behaviorTreeRoot != null && !behaviorTreeRoot.IsTerminated())
        {
            behaviorTreeRoot.Tick();
        }
    }
    public override void FixedUpdate(float dt)
    {
        if (behaviorTreeRoot != null) behaviorTreeRoot.FixedTick();
    }


}


}