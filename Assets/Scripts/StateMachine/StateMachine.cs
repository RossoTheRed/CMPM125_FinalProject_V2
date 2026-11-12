using System;
using System.Collections.Generic;
using UnityEngine;

// inspired by some of the code from the repo: https://github.com/kultie/Unity-State-Machine/tree/master
// Tooks snippets, but completely overhauled and compressed excessive chunks for my needs
// Note (Personal issue with repo): Restricted to only one update loop, meaning it could only ever be
// fixedupdate or update, not both
public class StateMachine : MonoBehaviour
{
    private Dictionary<Enum, StateComponent> states = new();
    private StateComponent currentState;

    // way to get around unity's constructor system to guarantee proper awaking procedure
    protected virtual void Init(params (Enum id, StateComponent state)[] entries)
    {
        foreach (var (id, state) in entries) states[id] = state;
    }

    // Lambda expressions to call the respective updates in unity
    void Update()       => currentState?.Update(Time.deltaTime);
    void FixedUpdate()  => currentState?.FixedUpdate(Time.fixedDeltaTime);

    // Transitions to a new state if one exists
    public void TransitionState(Enum next)
    {
        StateComponent nextState; // is there a way to shift this later? - chris
        // safe break or change to crash? Up to debate, ask prof what he wants herre - chris
        if (!states.TryGetValue(next, out nextState))
        {
            Debug.Log($"Failed State Transition: {next}");
            return;
        }
        // transition process
        currentState?.Exit(nextState);
        nextState.Enter(currentState);
        
        currentState = nextState;
    }

    // Check the current state they are in
    public StateComponent GetState()
    {
        return currentState;
    }

}