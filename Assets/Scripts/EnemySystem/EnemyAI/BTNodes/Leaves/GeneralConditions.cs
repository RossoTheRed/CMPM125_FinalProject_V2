using System;
using Unity.VisualScripting;
using UnityEngine;


namespace EnemyBT.Conditions
{

    public class GeneralConditions
    {
        public static bool HasLineOfSight(Transform source, Transform target, float maxDistance, LayerMask targetMask, LayerMask collisionMask)
        {
            Vector3 sourcePos = source.position;
            Vector3 targetPos = target.position;
            Vector3 direction = (targetPos - sourcePos).normalized;
            if (Physics.Raycast(sourcePos, direction, out RaycastHit hit, maxDistance, collisionMask))
            {
                if (hit.collider.gameObject.layer == targetMask) return true;
            }
            return false;
        }

        
    }

}