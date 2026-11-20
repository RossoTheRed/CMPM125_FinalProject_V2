using UnityEngine;

using PSM = PlayerStateMachine;
using States = PlayerStateMachine.StateId;

public class SneakState : StateComponent<PlayerStateMachine>
{
    PlayerInputChecker inputChecker;

    PSM.ControllerData data = new PSM.ControllerData { height = 0.6f, moveSpeed = 6 };
    
    bool exitCond1;
    bool exitCond2;

    public SneakState(PlayerStateMachine machine) : base(machine)
    {
        inputChecker = stateMachine.InputChecker;

    }
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Sneak!");
        stateMachine.UpdatePlayerController(data);
        
        
    }
    public override void Exit(StateComponent nextState) {}

    public override void Update(float dt)
    {
        exitCond1 = (inputChecker.crouchKey.IsPressed() == false && stateMachine.playerController.isCramped == false);
        exitCond2 = stateMachine.playerController.isGrounded == false; // Temporarily disregard because crouching currently puts the play in the air

        if (exitCond1) // || exitCond2)
        {
            stateMachine.TransitionState(States.Idle);
        }
    }
    public override void FixedUpdate(float dt)
    {
        
    }
}
