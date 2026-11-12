using UnityEngine;
using System;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] Animator animator;
    public int currState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currState != (int)controller.movementState)
        {
            currState = (int)controller.movementState;
            animator.SetInteger("movementState",currState);
        }
    }
}
