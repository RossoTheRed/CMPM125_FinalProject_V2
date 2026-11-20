using UnityEngine;

using PSM = PlayerStateMachine;
using States = PlayerStateMachine.StateId;

public class SprintState : StateComponent<PlayerStateMachine>
{
    PlayerInputChecker inputChecker;

    PSM.ControllerData data = new PSM.ControllerData { moveSpeed = 18 };

    public SprintState(PlayerStateMachine machine) : base(machine)
    {
        inputChecker = stateMachine.InputChecker;
    }
    public override void Enter(StateComponent previousState)
    {
        Debug.Log("In Sprint!");
        stateMachine.UpdatePlayerController(data);
    }
    public override void Exit(StateComponent nextState) {}
    public override void Update(float dt)
    {
        if (inputChecker.sprintKey.IsPressed() == false)
        {
            stateMachine.TransitionState(States.Idle);
        }
    }
    public override void FixedUpdate(float dt)
    {
        
    }
}
