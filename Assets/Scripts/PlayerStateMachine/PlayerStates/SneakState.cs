using UnityEngine;
using UnityEngine.InputSystem;

/**
 * The player's crouching state. Affects height and movement speed.
 */

public class SneakState : StateComponent<PlayerStateMachine>
{
    int moveSpeed = 6;
    float height = 0.6f;

    public SneakState(PlayerStateMachine machine) : base(machine)
    {
        
    }
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Sneak!");
        stateMachine.SetMoveSpeed(moveSpeed);
        stateMachine.SetHeight(height);
    }
    public override void Exit(StateComponent nextState) {}
    public override void Update(float dt)
    {

    }
    public override void FixedUpdate(float dt)
    {
        
    }
}
