using UnityEngine;
using EnemyBT;
using EnemyBT.Tree;
using EnemyDirectorStates;
using System;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class EnemyDirectorFSM : StateMachine
{
   [Header("References", order = 1)]
    [SerializeField] protected GameObject _target;
    public virtual GameObject target => _target;
    public LayerMask targetMask {get; private set;}
    public LayerMask collisionSightMask {get; private set;}
    [SerializeField] protected UniversalEnemyBlackboard _universalEnemyBlackboard;
    public virtual UniversalEnemyBlackboard universalEnemyBlackboard => _universalEnemyBlackboard; 
    [SerializeField] protected LayoutGenerator _layoutGenerator;
    private LayoutGenerator layoutGenerator => _layoutGenerator; 
    [SerializeField] protected NavMeshSurface _navMeshSurface;
    private NavMeshSurface navMeshSurface => _navMeshSurface;

    // Enemy Settings
    [Header("Settings", order = 2)]

    [SerializeField] protected int _sightDist = 10;
    public virtual int sightDist => _sightDist; 

    
    

    // Required for state machine initialization
    public enum StateId { Disabled, Calm, Aggressive }
    public bool alive {get ; private set;} = true;
    //private Action SetupGridAction;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        Debug.Log("enemy director is awake");
        EnemyBlackboard enemyBlackboard = universalEnemyBlackboard.enemyBlackboard;  
        enemyBlackboard.SetEnemyDirector(this);

        Init(
            (StateId.Disabled, new DisabledDirState(null, this)),
            (StateId.Calm, new CalmState(new CalmBT(enemyBlackboard), this)),
            (StateId.Aggressive, null)
        );
        TransitionState(StateId.Calm); // initial state

        targetMask = LayerMask.NameToLayer("Player");
        collisionSightMask = -1;

        Debug.Log(EventBus.Instance.GetHashCode());

        EventBus.Instance.Register(SetupGridData, EventBus.EventName.ProceduralDone);
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

    public void SetupGridData()
    {
        Debug.Log("Setting Grid");
        universalEnemyBlackboard.enemyBlackboard.SetGrid(layoutGenerator.Grid, universalEnemyBlackboard.enemyBlackboard.GetRoomSize());
        //navMeshSurface.collectObjects(navMeshSurface.children)
        navMeshSurface.BuildNavMesh();
        universalEnemyBlackboard.enemyBlackboard.enemyFSM.GetComponent<NavMeshAgent>().Warp(universalEnemyBlackboard.enemyBlackboard.enemyFSM.transform.position);
        
        universalEnemyBlackboard.enemyBlackboard.enemyFSM.GetComponent<NavMeshAgent>().enabled = true;
        //navMeshSurface.BuildNavMesh();
        

        EventBus.Instance.Deregister(SetupGridData, EventBus.EventName.ProceduralDone);
    
    }
}
