using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : StateMachine
{
    [SerializeField] private GameObject playerCamera;

    public InputActionMap fighterActionMap;
    // Movement Input Actions
    public InputAction movePlaneAction; // Note: Might be bad idea to set value here - chris

    public Hittable hp {get ; private set;}
    public Damage damage {get ; private set;}


    // example for speed and safety of data
    public int maxSpeed {get ; private set;} = 1;
    // or this which might be much better 
    [SerializeField] protected int _maxHealth = 10;
    public virtual int maxHealth => _maxHealth; 
    

    // Required for state machine initialization
    public enum StateId { Idle, Active } // feel free to add more
    public bool alive {get ; private set;} = true;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        Init(
            (StateId.Idle, new IdleState(this)),
            (StateId.Active, null)
        );
        TransitionState(StateId.Idle); // initial state

        hp = new Hittable(maxHealth, Hittable.Team.ENEMY, this.gameObject);

        hp.OnDeath += OnDeath;
    }

    protected virtual void OnEnable()
    {
        if (hp != null) 
        {
            hp.SetCurrentHP(maxHealth); 
            hp.OnDeath += OnDeath;
        }
        alive = true;
    }

    protected virtual void OnDisable()
    {
        if (hp != null)
        {
            hp.OnDeath -= OnDeath;
        }
    }

    protected virtual void OnDeath()
    {
        if (!alive) return;
        alive = false;
    }

}
