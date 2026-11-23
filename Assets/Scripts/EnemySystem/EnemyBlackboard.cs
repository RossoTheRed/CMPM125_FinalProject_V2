using UnityEngine;

public class EnemyBlackboard
{
    public EnemyFSM enemyFSM {get ; private set;}
    public EnemyDirectorFSM enemyDirectorFSM {get ; private set;}
    public bool isBlackboardActive = false;



    public void SetEnemyFSM(EnemyFSM _FSM)
    {
        if (enemyFSM) return;
        enemyFSM = _FSM;
        if (enemyDirectorFSM) isBlackboardActive = true;
    }

    public void SetEnemyDirector(EnemyDirectorFSM _FSM)
    {
        if (enemyDirectorFSM) return;
        enemyDirectorFSM = _FSM;
        if (enemyFSM) isBlackboardActive = true;
    }
}
