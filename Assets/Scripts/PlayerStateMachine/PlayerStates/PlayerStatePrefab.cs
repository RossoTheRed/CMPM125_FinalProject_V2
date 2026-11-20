using UnityEngine;

using PSM = PlayerStateMachine;
using States = PlayerStateMachine.StateId;

public class PlayerState : StateComponent<PSM>
{
    // This is called at state initialization
    public PlayerState(PSM machine) : base(machine)
    {

    }

    // This is called every time you transition to this state
    public override void Enter(StateComponent previousState)
    {

    }

    // This is called every time you transition out of this state
    public override void Exit(StateComponent nextState) { }

    // This is called every frame you stay in this state
    public override void Update(float dt)
    {

    }

    // This is called every physics update you stay in this state
    public override void FixedUpdate(float dt)
    {

    }
}
