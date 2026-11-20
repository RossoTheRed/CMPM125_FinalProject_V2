using UnityEngine;

using PSM = PlayerStateMachine;

public class IdleState : StateComponent<PlayerStateMachine>
{
    PlayerInputChecker inputChecker;
    StaminaController staminaController;

    PSM.ControllerData data = new PSM.ControllerData{ height = 1, moveSpeed = 10};

    // This is called at state initialization
    public IdleState(PlayerStateMachine machine) : base(machine)
    {
        inputChecker = stateMachine.InputChecker;
        staminaController = stateMachine.StaminaController;
    }

    // This is called every time you transition to this state
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Idle!");
        stateMachine.UpdatePlayerController(data);
        staminaController.DegradeMaximum_Start();
        staminaController.Regen_Start();
    }

    // This is called every time you transition out of this state
    public override void Exit(StateComponent nextState) {}

    // This is called every frame you stay in this state
    public override void Update(float dt)
    {
        if (inputChecker.sprintKey.WasPressedThisFrame())
        {
            stateMachine.TransitionState(PSM.StateId.Sprint);
        } else if (inputChecker.crouchKey.WasPressedThisFrame())
        {
            stateMachine.TransitionState(PSM.StateId.Sneak);
        }
    }

    // This is called every physics update you stay in this state
    public override void FixedUpdate(float dt)
    {
        
    }
}
