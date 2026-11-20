using UnityEngine;
using UnityEngine.InputSystem;
public class SprintState : StateComponent<PlayerStateMachine>
{
    int moveSpeed = 18;
    float height = 1;

    public SprintState(PlayerStateMachine machine) : base(machine)
    {
        
    }
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Sprint!");
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
