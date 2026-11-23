using EnemyBT.Tree;
using UnityEngine;

namespace EnemyStates {

public class PatrolState : StateComponent<EnemyFSM>
{
    private EnemyBaseBT behaviorTree;
    public PatrolState(EnemyBaseBT bt, EnemyFSM machine) : base(machine)
    {
        behaviorTree = bt;
        behaviorTree.OnInitialize();
    }

    public override void Enter(StateComponent previousState)
    {
        behaviorTree.OnInitialize();
    }
    public override void Exit(StateComponent nextState) {}
    public override void Update(float dt)
    {
        behaviorTree.Tick();
    }
    public override void FixedUpdate(float dt)
    {
        behaviorTree.FixedTick();
    }


}


}