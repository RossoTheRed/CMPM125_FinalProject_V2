using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectCheck : MonoBehaviour
{
    [SerializeField] string layerToCheck;
    private bool blocked;

    // void OnTriggerEnter(Collider hit)
    // {
    //     if (hit.gameObject.layer == LayerMask.NameToLayer(layerToCheck))
    //     {
    //         blocked = true;
    //     }
    //     Debug.Log($"Hit entered {hit.transform.name}");
    // }

    void OnTriggerStay(Collider hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer(layerToCheck))
        {
            blocked = true;
        } else
        {
            blocked = false;
        }
    }

    void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.layer == LayerMask.NameToLayer(layerToCheck))
        {
            blocked = false;
        }
    }

    public bool IsBlocked()
    {
        return(blocked);
    }
}
