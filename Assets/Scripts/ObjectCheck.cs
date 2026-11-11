using System;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectCheck : MonoBehaviour
{
    private bool blocked;

    void OnTriggerEnter()
    {
        blocked = true;
    }

    void OnTriggerExit()
    {
        blocked = false;
    }

    public bool IsBlocked()
    {
        return(blocked);
    }
}
