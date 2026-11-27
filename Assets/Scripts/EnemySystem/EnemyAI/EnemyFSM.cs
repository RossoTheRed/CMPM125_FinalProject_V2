using System.Collections.Generic;
using System.IO;
using EnemyBT.Tree;
using EnemyStates;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFSM : StateMachine
{
    [Header("References", order = 1)]
    [SerializeField] protected GameObject _target;
    public virtual GameObject target => _target; 
    [SerializeField] protected NavMeshAgent _navAgent;
    public virtual NavMeshAgent navAgent => _navAgent;  
    public bool hasNavPath {get; private set;} = false;
    public NavMeshPath path;
    public Rigidbody sourceRB {get; private set;}
    public Hittable hp {get ; private set;}
    public LayerMask targetMask {get; private set;}
    public LayerMask collisionSightMask {get; private set;}
    private EnemyBlackboard enemyBlackboard;

    // Enemy Settings
    [Header("Settings", order = 2)]
    [SerializeField] protected int _maxHealth = 10;
    public virtual int maxHealth => _maxHealth; 

    [SerializeField] protected int _maxSpeed = 10;
    public virtual int maxSpeed => _maxSpeed; 

    [SerializeField] protected int _sightDist = 10;
    public virtual int sightDist => _sightDist; 
    [SerializeField] protected int _rotateSpeed = 1;
    public virtual int rotateSpeed => _rotateSpeed; 

    
    

    // Required for state machine initialization
    public enum StateId { Disabled, Patrol, Chase }
    public bool alive {get ; private set;} = true;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        enemyBlackboard = new EnemyBlackboard();
        enemyBlackboard.SetEnemyFSM(this);  

        Init(
            (StateId.Disabled, new DisabledState(null, this)),
            (StateId.Patrol, new PatrolState(new PatrolBT(enemyBlackboard), this)),
            (StateId.Chase, null)
        );
        TransitionState(StateId.Patrol); // initial state

        sourceRB = GetComponent<Rigidbody>();
        hp = new Hittable(maxHealth, Hittable.Team.ENEMY, this.gameObject);
        targetMask = LayerMask.NameToLayer("Player");
        collisionSightMask = -1;

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

    protected void ResetValidPath()
    {
        if (path.status == NavMeshPathStatus.PathComplete && hasNavPath != false)
        {
            hasNavPath = false;
        }
    }

    public Vector2Int GetCurrentRoom()
    {
        return new Vector2Int(0, 0);
    }
}