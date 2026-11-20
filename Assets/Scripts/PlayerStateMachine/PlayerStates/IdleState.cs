using UnityEngine;
using UnityEngine.InputSystem;
public class IdleState : StateComponent<PlayerStateMachine>
{
    int moveSpeed = 10;
    float height = 1;

    // This is called at state initialization
    public IdleState(PlayerStateMachine machine) : base(machine)
    {
        
    }

    // This is called every time you transition to this state
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Idle!");
        stateMachine.SetMoveSpeed(moveSpeed);
        stateMachine.SetHeight(height);
    }

    // This is called every time you transition out of this state
    public override void Exit(StateComponent nextState) {}

    // This is called every frame you stay in this state
    public override void Update(float dt)
    {
        
    }

    // This is called every physics update you stay in this state
    public override void FixedUpdate(float dt)
    {
        
    }
}
