using Photon.Pun;
using System;
using UnityEngine;

public class Tree : MonoBehaviour {
    private Health health;
    private TreeAbility ability;
    [SerializeField]
    private Animator animator;

    private Action onDeath;

    [SerializeField]
    private float actionCooldown;
    private float timeUntilNextAction;

    private void Awake() {
        health = GetComponent<Health>();
        health.Init(1);
        health.AddHealthListener(CheckHealth);
    }

    public void Init(TreeStatblock stats) {
        actionCooldown = stats.Cooldown;
        timeUntilNextAction = actionCooldown;
        ability = gameObject.AddComponent<RangedAttackAbility>();
        ability.Init(stats);
    }

    private void Update() {
        timeUntilNextAction -= Time.deltaTime;
        if(timeUntilNextAction <= 0) {
            bool didAbility = ability.TryPerform();
            if(didAbility) {
                animator.SetTrigger("Attack");
                timeUntilNextAction = actionCooldown;
            }
        }
    }

    public void AddDeathListener(Action deathListener) {
        onDeath += deathListener;
    }

    private void CheckHealth(float health, float maxHealth) {
        if(health <= 0) {
            onDeath?.Invoke();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
