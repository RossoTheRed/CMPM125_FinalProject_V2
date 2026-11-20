using UnityEngine;
using System.Collections;

using PSM = PlayerStateMachine;

public class SprintState : StateComponent<PlayerStateMachine>
{
    PlayerInputChecker inputChecker;
    StaminaController staminaController;

    PSM.ControllerData data = new PSM.ControllerData { moveSpeed = 18 };
    float stamDrainAmount = 0.25f;
    float stamDrainInterval = 0.25f;

    public SprintState(PlayerStateMachine machine) : base(machine)
    {
        inputChecker = stateMachine.InputChecker;
        staminaController = stateMachine.StaminaController;
    }
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Sprint!");
        stateMachine.UpdatePlayerController(data);
        staminaController.Drain_Start(this.stamDrainAmount, this.stamDrainInterval);
    }
    public override void Exit(StateComponent nextState)
    {
        staminaController.Drain_Stop();
    }
    public override void Update(float dt)
    {
        if (inputChecker.sprintKey.IsPressed() == false)
        {
            stateMachine.TransitionState(PSM.StateId.Idle);
        }

        if (inputChecker.moveKeys.WasReleasedThisFrame() && inputChecker.moveKeys.IsPressed() == false)
        {
            staminaController.Drain_Stop();
        } else if (inputChecker.moveKeys.WasPressedThisFrame() && inputChecker.moveKeys.IsPressed())
        {
            staminaController.Drain_Start(this.stamDrainAmount, this.stamDrainInterval);
        }
    }
    public override void FixedUpdate(float dt)
    {
        
    }
}
