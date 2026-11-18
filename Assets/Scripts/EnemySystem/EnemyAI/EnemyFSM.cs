using System.Collections.Generic;
using UnityEngine;

public class EnemyFSM : StateMachine
{
    [Header("References", order = 1)]
    [SerializeField] protected List<Behavior> _behaviorTreeRoots = new List<Behavior>();
    public virtual List<Behavior> behaviorTreeRoots => _behaviorTreeRoots; 
    public Hittable hp {get ; private set;}

    // Enemy Settings
    [Header("Settings", order = 2)]
    [SerializeField] protected int _maxHealth = 10;
    public virtual int maxHealth => _maxHealth; 

    [SerializeField] protected int _maxSpeed = 10;
    public virtual int maxSpeed => _maxSpeed; 


    

    // Required for state machine initialization
    public enum StateId { Disabled, Patrol, Chase }
    public bool alive {get ; private set;} = true;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        Init(
            (StateId.Disabled, null),
            (StateId.Patrol, null),
            (StateId.Chase, null)
        );
        TransitionState(StateId.Disabled); // initial state

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