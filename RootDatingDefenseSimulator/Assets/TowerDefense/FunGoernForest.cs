using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Health))]
public class FunGoernForest : MonoBehaviour {

    private Health health;

    private Action onDeath;

    public void Init(Action gameOverListener) {
        health = GetComponent<Health>();
        health.AddHealthListener(CheckDeath);
        AddDeathListener(gameOverListener);
        Enemy.RegisterStaticTarget(health);
    }

    public void AddDeathListener(Action onDeath) {
        this.onDeath += onDeath;
    }

    private void CheckDeath(float health, float maxHealth) {
        if(health <= 0) {
            Debug.Log($"Forest died! D:");
            Enemy.DeregisterTarget(this.health);
            onDeath?.Invoke();
        }
    }
}
