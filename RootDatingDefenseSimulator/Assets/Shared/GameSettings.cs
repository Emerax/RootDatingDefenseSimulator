using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject {
    [Header("Health Settings")]
    public float funGoernForestHealth;
    public float rockHealth;
    public float treeHealth;

    [Header("Wave Settings")]
    public float timeBetweenWaves;
    public float timeBetweenEnemySpawns;

    [Header("Other Settings")]
    public float minTreeScaleFactorFromLifeLoss;
    public float treeLifetime;
    public float projectileLifetime;
}
