using UnityEngine;
using EnemyBT;

public class EnemyDirectorFSM : StateMachine
{
   [Header("References", order = 1)]
    [SerializeField] protected GameObject _target;
    public virtual GameObject target => _target;
    public LayerMask targetMask {get; private set;}
    public LayerMask collisionSightMask {get; private set;}
    private EnemyBlackboard enemyBlackboard;

    // Enemy Settings
    [Header("Settings", order = 2)]

    [SerializeField] protected int _sightDist = 10;
    public virtual int sightDist => _sightDist; 

    
    

    // Required for state machine initialization
    public enum StateId { Disabled, Patrol, Chase }
    public bool alive {get ; private set;} = true;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        enemyBlackboard = new EnemyBlackboard();
        enemyBlackboard.SetEnemyDirector(this);  

        Init(
            (StateId.Disabled, new DisabledState(null, this)),
            (StateId.Patrol, new PatrolState(new PatrolBT(enemyBlackboard), this)),
            (StateId.Chase, null)
        );
        TransitionState(StateId.Patrol); // initial state

        targetMask = LayerMask.NameToLayer("Player");
        collisionSightMask = -1;

    }


    protected virtual void OnEnable()
    {
        alive = true;
    }

    protected virtual void OnDisable()
    {
        
    }

    protected virtual void OnDeath()
    {
        if (!alive) return;
        alive = false;
    }
}
