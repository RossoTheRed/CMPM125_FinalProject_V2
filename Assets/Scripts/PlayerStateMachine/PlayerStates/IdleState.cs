using UnityEngine;

using PSM = PlayerStateMachine;
using States = PlayerStateMachine.StateId;

public class IdleState : StateComponent<PlayerStateMachine>
{
    PlayerInputChecker inputChecker;

    PSM.ControllerData data = new PSM.ControllerData{ height = 1, moveSpeed = 10};

    // This is called at state initialization
    public IdleState(PlayerStateMachine machine) : base(machine)
    {
        inputChecker = stateMachine.InputChecker;
    }

    // This is called every time you transition to this state
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Idle!");
        stateMachine.UpdatePlayerController(data);
    }

    // This is called every time you transition out of this state
    public override void Exit(StateComponent nextState) {}

    // This is called every frame you stay in this state
    public override void Update(float dt)
    {
        if (inputChecker.sprintKey.WasPressedThisFrame())
        {
            stateMachine.TransitionState(States.Sprint);
        } else if (inputChecker.crouchKey.WasPressedThisFrame())
        {
            stateMachine.TransitionState(States.Sneak);
        }
    }

    // This is called every physics update you stay in this state
    public override void FixedUpdate(float dt)
    {
        
    }
}
