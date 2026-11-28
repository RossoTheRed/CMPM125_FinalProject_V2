using UnityEngine;
using UnityEngine.AI;

namespace EnemyBT.Actions
{
public static class GeneralActions
{
    public static void RotateTowards(Rigidbody source, Transform target, int step)
    {
        Vector3 normalizedDirection = (target.position - source.transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(normalizedDirection, Vector3.up);
        source.rotation = Quaternion.RotateTowards(source.transform.rotation, rotation, step);
    }
    public static void RotateTowardsDir(Rigidbody source, float step)
    {
        Vector3 moveDirection = source.linearVelocity;
        moveDirection.y = 0f;
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            source.MoveRotation(Quaternion.RotateTowards(source.rotation, targetRotation, step));
        }

        
        //source.rotation = Quaternion.RotateTowards(source.transform.rotation, rotation, step);
    }

    public static void BasicHalt(Rigidbody source, int maxSpeed)
    {
        Vector3 direction = Vector3.zero;
        direction.Set(direction.x, direction.y, direction.z);
        Vector3 velocityChange = (direction * maxSpeed) - source.linearVelocity;
        source.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public static void BasicChase(Rigidbody source, Transform target, int maxSpeed)
    {
        Vector3 direction = (target.position - source.transform.position).normalized;
        direction.Set(direction.x, direction.y, direction.z);
        Vector3 velocityChange = (direction * maxSpeed) - source.linearVelocity;
        source.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public static void BasicChase(Rigidbody source, Vector3 targetPos, int maxSpeed)
    {
        Vector3 direction = (targetPos - source.transform.position).normalized;
        direction.Set(direction.x, direction.y, direction.z);
        Vector3 velocityChange = (direction * maxSpeed) - source.linearVelocity;
        source.AddForce(velocityChange, ForceMode.Acceleration);
    }

    public static bool FindValidPosition(EnemyBlackboard enemyBlackboard, Vector3 location, out Vector3 finalLocation, out NavMeshPath navPath)
    {
        finalLocation = Vector3.zero;
        navPath = null;
        if (enemyBlackboard.enemyFSM.hasNavPath) return false;
        if (!NavMesh.SamplePosition(location, out NavMeshHit targetHit, enemyBlackboard.enemyFSM.maxNavDist, -1))
        {
            return false;
        }

        NavMeshPath path = new NavMeshPath();
        if (!NavMesh.CalculatePath(
            enemyBlackboard.enemyFSM.transform.position,
            targetHit.position,
            -1,
            path))
        {
            return false;
        }

        navPath = path;

        finalLocation = targetHit.position;
        return true;
    }

}
}
