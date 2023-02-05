using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyVariants", menuName = "ScriptableObjects/EnemyVariants", order = 1)]
public class EnemyVariants : ScriptableObject {
    public List<EnemyVariant> variants;

    private Dictionary<EnemyVariantID, EnemyStats> enemyIdsToStats = null;
    public EnemyStats GetVariantStats(EnemyVariantID id, int waveIndex) {
        if(enemyIdsToStats == null) {
            enemyIdsToStats = new Dictionary<EnemyVariantID, EnemyStats>();
            foreach(EnemyVariant variant in variants) {
                enemyIdsToStats.Add(variant.id, variant.stats);
            }
            foreach(EnemyVariantID variantId in Enum.GetValues(typeof(EnemyVariantID))) {
                if(!enemyIdsToStats.ContainsKey(variantId)) {
                    throw new Exception($"Stats for {nameof(EnemyVariantID)} {variantId} is missing in {nameof(EnemyVariants)}");
                }
            }
        }
        return enemyIdsToStats[id].WaveStats(waveIndex);
    }
}

public enum EnemyVariantID {
    Weak = 0,
    Strong = 1,
    Fast = 2,
    FlyingWeak = 3,
    FlyingStrong = 4,
    FlyingFast = 5,
    BigBoss = 6,
}

[Serializable]
public class EnemyVariant {
    public EnemyVariantID id;
    public EnemyStats stats;
}

[Serializable]
public class EnemyStats {
    public float movementSpeed;
    public float health;
    public float attackDamage;
    public float verticalOffset;
    [Header("Increase per wave")]
    public float increaseMovementSpeed;
    public float increaseHealth;
    public float increaseAttackDamage;

    public EnemyStats WaveStats(int waveIndex) {
        EnemyStats stats = (EnemyStats)MemberwiseClone();
        stats.movementSpeed += increaseMovementSpeed * waveIndex;
        stats.health += increaseHealth * waveIndex;
        stats.attackDamage += increaseAttackDamage * waveIndex;
        return stats;
    }

    public object[] ToObjectArray() {
        return new[]{ (object)movementSpeed, health, attackDamage, verticalOffset, increaseMovementSpeed, increaseHealth, increaseAttackDamage };
    }

    public static EnemyStats FromObjectArray(object[] array) {
        EnemyStats stats = new();
        stats.movementSpeed = (float)array[0];
        stats.health = (float)array[1];
        stats.attackDamage = (float)array[2];
        stats.verticalOffset = (float)array[3];
        stats.increaseMovementSpeed = (float)array[4];
        stats.increaseHealth = (float)array[5];
        stats.increaseAttackDamage = (float)array[6];
        return stats;
    }
}