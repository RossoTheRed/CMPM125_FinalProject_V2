using UnityEngine;

public class IdleState : StateComponent<PlayerStateMachine>
{
    public IdleState(PlayerStateMachine machine) : base(machine)
    {
        Debug.Log(stateMachine.maxHealth); // example of how to access variables in the state machine

    }
    public override void Enter(StateComponent previousState) {}
    public override void Exit(StateComponent nextState) {}
    public override void Update(float dt)
    {
        
    }
    public override void FixedUpdate(float dt)
    {
        
    }


}
