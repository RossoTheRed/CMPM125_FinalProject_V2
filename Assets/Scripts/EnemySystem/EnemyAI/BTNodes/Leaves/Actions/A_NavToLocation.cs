using EnemyBT.Actions;
using UnityEngine;
using UnityEngine.AI;

public class NavToLocation : ActionNode
{
    private EnemyBlackboard enemyBlackboard;
    
    public NavToLocation(EnemyBlackboard enemyBlackboard) : base() 
    {
        this.enemyBlackboard = enemyBlackboard;
    }
    
    protected override Status Update() 
    {
        // because there is a fixed update, it should go and commit those actions
        return Status.BH_SUCCESS;
    }

    protected override Status FixedUpdate()
    {
        
        GeneralActions.RotateTowards(enemyBlackboard.enemyFSM.sourceRB, enemyBlackboard.enemyFSM.target.transform, enemyBlackboard.enemyFSM.rotateSpeed);
        Vector3 finalLocation = Vector3.zero;
        
        bool status = FindValidPosition(enemyBlackboard.enemyFSM.target.transform.position, out finalLocation);
        if (status)  
        {
            Debug.Log("moving");
            GeneralActions.BasicChase(enemyBlackboard.enemyFSM.sourceRB, finalLocation, enemyBlackboard.enemyFSM.maxSpeed);
        }
        else GeneralActions.BasicChase(enemyBlackboard.enemyFSM.sourceRB, enemyBlackboard.enemyFSM.transform.position, enemyBlackboard.enemyFSM.maxSpeed);
        return Status.BH_RUNNING;
    }

    protected bool FindValidPosition(Vector3 location, out Vector3 finalLocation)
    {
        finalLocation = Vector3.zero;
        if (enemyBlackboard.enemyFSM.hasNavPath) return false;
        float radius = 2f;
        if (!NavMesh.SamplePosition(location, out NavMeshHit targetHit, radius, enemyBlackboard.enemyFSM.collisionSightMask))
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        if (!NavMesh.CalculatePath(
            enemyBlackboard.enemyFSM.transform.position,
            targetHit.position,
            enemyBlackboard.enemyFSM.collisionSightMask,
            path))
        {
            return false;
        }

        enemyBlackboard.enemyFSM.path = path;

        finalLocation = targetHit.position;
        return true;
    }
}
