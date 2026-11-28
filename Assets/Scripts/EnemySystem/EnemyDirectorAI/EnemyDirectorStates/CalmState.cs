
using System;
using System.Collections;
using EnemyBT.Tree;
using UnityEngine;

namespace EnemyDirectorStates {

public class CalmState : StateComponent<EnemyDirectorFSM>
{
    private EnemyBaseBT behaviorTree;
    private bool isWaitActive = false;
    private bool isTickActive = false;
    private IEnumerator slowerTickLoop;
    public CalmState(EnemyBaseBT bt, EnemyDirectorFSM machine) : base(machine)
    {
        behaviorTree = bt;
        behaviorTree.OnInitialize();
    }

    // Initialize 
    // Will start up the enemy behavior tree if one exists
    public override void Enter(StateComponent previousState)
    {
        behaviorTree.OnInitialize();
    }
    public override void Exit(StateComponent nextState) {}
    public override void Update(float dt)
    {
        if(isTickActive) behaviorTree.Tick();
    }
    public override void FixedUpdate(float dt)
    {
        if(isTickActive) behaviorTree.FixedTick();
        StartSlowerTickLoop();

    }

    // Starts the timer, and resets it if started again
        private void StartSlowerTickLoop()
        {
            if (isWaitActive) {
                return;
            }
            isWaitActive = true;
            slowerTickLoop = SlowerTickLoop();
            CoroutineManager.Instance.Run(slowerTickLoop);
        }

        private void StopSlowerTickLoop()
        {
            if (!isWaitActive || CoroutineManager.Instance == null) return;
            CoroutineManager.Instance.Stop(slowerTickLoop);
            slowerTickLoop = null;
            isWaitActive = false;
        }

    public IEnumerator SlowerTickLoop()
    {
        yield return new WaitForSeconds(5f);
        isTickActive = true;
        yield return new WaitForFixedUpdate();
        isTickActive = false;
        StopSlowerTickLoop();
    }

}


}