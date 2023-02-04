using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject {
    [Header("Health Settings")]
    public float funGoernForestHealth;
    public float rockHealth;
    public float treeHealth;
    public float enemyHealth;

    [Header("Damage Settings")]
    public float treeRangedAttackDamage;
    public float enemyDamage;

    [Header("Spawn Settings")]
    public float timeBetweenEnemySpawns;
}
