using UnityEngine;

[CreateAssetMenu(fileName = "UniversalEnemyBlackboard", menuName = "Scriptable Objects/UniversalEnemyBlackboard")]
public class UniversalEnemyBlackboard : ScriptableObject
{
    public EnemyBlackboard enemyBlackboard = new EnemyBlackboard();
}
