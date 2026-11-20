using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : StateMachine
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private PlayerController _playerController;
    public PlayerController playerController {get { return(_playerController); }}
    [SerializeField] PlayerInputChecker _inputChecker;
    public PlayerInputChecker InputChecker {get { return(_inputChecker); }}
    [SerializeField] StaminaController _staminaController;
    public StaminaController StaminaController { get { return (_staminaController); } }

    public Hittable hp {get ; private set;}
    public Damage damage {get ; private set;}


    // example for speed and safety of data
    public float moveSpeed {get ; private set;} = 1;
    public float height { get; private set; } = 1;
    // or this which might be much better 
    [SerializeField] protected int _maxHealth = 10;
    public virtual int maxHealth => _maxHealth;

    public class ControllerData
    {
        public float ?height;
        public float ?moveSpeed;
    }

    // Required for state machine initialization
    public enum StateId { Idle, Sneak, Sprint } // feel free to add more
    public bool alive {get ; private set;} = true;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        Init(
            (StateId.Idle, new IdleState(this)),
            (StateId.Sneak, new SneakState(this)),
            (StateId.Sprint, new SprintState(this))
        );
        TransitionState(StateId.Idle); // initial state

        hp = new Hittable(maxHealth, Hittable.Team.ENEMY, this.gameObject);

        hp.OnDeath += OnDeath;

        _playerController.inputChecker = this._inputChecker;
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

    public void SetMoveSpeed(float newSpeed)
    {
        Debug.Log($"moveSpeed: {moveSpeed} -> {newSpeed}");
        moveSpeed = newSpeed;
        _playerController.ChangeSpeed(moveSpeed/100);
    }

    public void SetHeight(float newHeight)
    {
        Debug.Log($"height: {height} -> {newHeight}");
        height = newHeight;
        _playerController.ChangeHeight(height);
    }

    public void UpdatePlayerController(ControllerData data)
    {
        // Crude way first, then refactor later whn I know how
        if (data.height != null)
        {
            SetHeight((float) data.height);
        }

        if (data.moveSpeed != null)
        {
            SetMoveSpeed((float) data.moveSpeed);
        }
    }
}
