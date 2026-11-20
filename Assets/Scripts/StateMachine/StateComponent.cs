using System;

// inspired by some of the code from the repo: https://github.com/kultie/Unity-State-Machine/tree/master
// Tooks snippets, but completely overhauled and compressed excessive chunks for my needs

public abstract class StateComponent
{
    // maybe make this read only, but probably not - chris
    protected StateMachine stateMachine; // one link set by the state machine upon instantiation/creation

    // maybe provide interface to remove need of public access
    public abstract void Enter(StateComponent previousState);
    public abstract void Exit(StateComponent nextState);
    public virtual void Update(float dt) {}
    public virtual void FixedUpdate(float dt) {}

}

// Note: patch this up, im not sure if this works, looking up the documentation
// meant going off assumptions, but this fixes issues of referencing stateMachine for variables
// in a new StateMachine which had inherited from StateMachine
public abstract class StateComponent<TMachine> : StateComponent
    where TMachine : StateMachine
{
    protected new readonly TMachine stateMachine;

    protected StateComponent(TMachine machine)
    {
        stateMachine = machine;
    }

    protected void ChangeState(Enum next)
    {
        stateMachine.TransitionState(next);
    }
}