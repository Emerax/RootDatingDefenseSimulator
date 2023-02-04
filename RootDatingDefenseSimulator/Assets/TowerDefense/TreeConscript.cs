using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DestructableObstacle))]
public class TreeConscript : MonoBehaviour {
    private Health health;
    private TreeAbility ability;
    [SerializeField]
    private GameSettings gameSettings;
    [SerializeField]
    private Animator animator;

    private Action onDeath;

    private float actionCooldown;
    private float timeUntilNextAction;

    private void Awake() {
        Assert.IsNotNull(gameSettings);
        health = GetComponent<Health>();
        health.Init(gameSettings.treeHealth);
        health.AddHealthListener(CheckHealth);
    }

    public void Init(TreeStatblock stats) {
        transform.localScale = stats.Size * transform.localScale;
        actionCooldown = stats.Cooldown;
        timeUntilNextAction = actionCooldown;
        ability = gameObject.AddComponent<RangedAttackAbility>();
        ability.Init(stats, gameSettings);
    }

    private void Update() {
        if(GameLogic.PlayerRole is not PlayerRole.TOWER_DEFENSER) {
            return;
        }

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
        }
    }
}
