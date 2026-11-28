using System.Collections.Generic;
using System.IO;
using EnemyBT.Actions;
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
    [SerializeField] protected UniversalEnemyBlackboard _universalEnemyBlackboard;
    public virtual UniversalEnemyBlackboard universalEnemyBlackboard => _universalEnemyBlackboard; 
    //private EnemyBlackboard enemyBlackboard;

    // Enemy Settings
    [Header("Settings", order = 2)]
    [SerializeField] protected int _maxHealth = 10;
    public virtual int maxHealth => _maxHealth; 

    [SerializeField] protected int _maxSpeed = 10;
    public virtual int maxSpeed => _maxSpeed; 
    [SerializeField] protected int _maxAccel = 10;
    public virtual int maxAccel => _maxAccel; 
    [SerializeField] protected int _rotateSpeed = 1;
    public virtual int rotateSpeed => _rotateSpeed; 
    [SerializeField] protected int _stopDist = 10;
    public virtual int stopDist => _stopDist;

    [SerializeField] protected int _sightDist = 10;
    public virtual int sightDist => _sightDist; 
    [SerializeField] protected float _maxNavDist = 2f;
    public virtual float maxNavDist => _maxNavDist; 


    // Enemy Tracking Variables
    private Vector3 navTargetPosition;
    private bool hasSetDestination;
    
    

    // Required for state machine initialization
    public enum StateId { Disabled, Patrol, Chase }
    public bool alive {get ; private set;} = true;

    // Awake
    // Sets up the states with proper connections to possible states
     protected virtual void Awake()
    {
        EnemyBlackboard enemyBlackboard = universalEnemyBlackboard.enemyBlackboard;
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

        // set variables for nav agent
        navAgent.angularSpeed = rotateSpeed;
        navAgent.speed = maxSpeed;
        navAgent.acceleration = maxAccel;
        navAgent.stoppingDistance = stopDist;

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

    /*public Vector2Int GetCurrentRoom()
    {
        EnemyBlackboard enemyBlackboard = universalEnemyBlackboard.enemyBlackboard;
        Vector2 position = new Vector2(transform.position.x, transform.position.z);
        Vector2Int gridPos = new Vector2Int((int)(position.x / enemyBlackboard.GetRoomSize()), (int) (position.y / enemyBlackboard.GetRoomSize()));
        
        return gridPos;
    }*/

    public Vector2Int GetCurrentRoom()
    {
        EnemyBlackboard enemyBlackboard = universalEnemyBlackboard.enemyBlackboard;
        float roomSize = enemyBlackboard.GetRoomSize();
        
        // Assuming your grid origin is stored as a Vector2 or Vector3 (X and Z)
        Vector2 gridOrigin = GameManager.Instance.bottomLeftCornerGridPosition;

        // Subtract the origin to get the local position relative to the bottom-left corner
        float localX = transform.position.x - gridOrigin.x;
        float localZ = transform.position.z - gridOrigin.y; // Use .y if origin is Vector2, or .z if Vector3

        //room size and floor to get index
        int gridX = Mathf.FloorToInt(localX / roomSize);
        int gridZ = Mathf.FloorToInt(localZ / roomSize);
        
        return new Vector2Int(gridX, gridZ);
    }

    public Status SetTargetPosition(Vector3 newTargetPos)
    {
        Vector3 newPos = Vector3.zero;
        // Checks if we should get a new target position
        if (!hasSetDestination)
        {
            if (GeneralActions.FindValidPosition(universalEnemyBlackboard.enemyBlackboard, newTargetPos, out newPos, out NavMeshPath navPath))
            {
                navAgent.SetDestination(newPos);
                this.path = navPath;
                hasSetDestination = true;
                navTargetPosition = newTargetPos;
            }
            else 
            {
                return Status.BH_FAILURE;
            }
        }
        else
        {
            if (Vector3.Distance(navTargetPosition, newTargetPos) > 0.1f)
            {
                if (GeneralActions.FindValidPosition(universalEnemyBlackboard.enemyBlackboard, newTargetPos, out newPos, out NavMeshPath navPath))
                {
                    navAgent.SetDestination(newPos);
                    this.path = navPath;
                    hasSetDestination = true;
                    navTargetPosition = newTargetPos;
                }
                else return Status.BH_FAILURE;
            }
        }

        // check if we are there yet, incase we want to remove current target position
        if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            DisableNavAgent();
            return Status.BH_SUCCESS;
        }

        // if everything passes, we need to go to the location
        Vector3 desiredVelocity = navAgent.desiredVelocity;

        if (desiredVelocity.sqrMagnitude > 0.01f)
        {
            // might move this to acceleration - chris
            Vector3 targetVelocity = desiredVelocity;
            sourceRB.linearVelocity = Vector3.MoveTowards(sourceRB.linearVelocity, targetVelocity, maxAccel * Time.fixedDeltaTime);
        }
        navAgent.nextPosition = sourceRB.position;
        
        return Status.BH_SUCCESS; // could be running, for now im doing success

    }

    public Vector3 GetNavTargetPosition()
    {
        return navTargetPosition;
    }
    public void EnableNavAgent()
    {
        navAgent.updatePosition = true;
    }

    public void DisableNavAgent()
    {
        hasSetDestination = false;
        navAgent.updatePosition = false;
    }
}