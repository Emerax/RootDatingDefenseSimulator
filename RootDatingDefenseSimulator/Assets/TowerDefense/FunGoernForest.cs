using System;
using UnityEngine;

public class FunGoernForest : MonoBehaviour {
    [SerializeField]
    private float maxHealth;

    private Health health;

    private Action onDeath;

    public void Init(Action gameOverListener) {
        health = GetComponent<Health>();
        health.Init(maxHealth);
        health.SetHealthListener(CheckDeath);
        AddDeathListener(gameOverListener);
    }

    public void AddDeathListener(Action onDeath) {
        this.onDeath += onDeath;
    }

    private void CheckDeath(float health, float maxHealth) {
        if(health <= 0) {
            onDeath?.Invoke();
        }
    }
}
