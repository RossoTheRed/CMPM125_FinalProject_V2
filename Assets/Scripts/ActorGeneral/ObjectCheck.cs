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
        Debug.Log($"Hit continued {hit.transform.name}");
    }

    void OnTriggerExit(Collider hit)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer(layerToCheck))
            {
                blocked = false;
            }
            Debug.Log($"Hit exitted {hit.transform.name}");
        }

    public bool IsBlocked()
    {
        return(blocked);
    }
}
