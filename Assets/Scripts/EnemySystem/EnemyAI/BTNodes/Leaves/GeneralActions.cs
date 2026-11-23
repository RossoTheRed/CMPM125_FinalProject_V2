using UnityEngine;

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

}
}
