using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DestructableObstacle))]
public class TreeConscript : MonoBehaviourPun {

    private Health health;
    private TreeAbility ability;
    [SerializeField]
    private GameSettings gameSettings;
    [SerializeField]
    private Animator animator;

    private Action onDeath;

    private float actionCooldown;
    private float timeUntilNextAction;
    private float timeUntilAutoDeath;
    private float initialScaleY;
    private bool isInitialized = false;

    private void Awake() {
        Assert.IsNotNull(gameSettings);
        health = GetComponent<Health>();
        health.Init(gameSettings.treeHealth);
        health.AddHealthListener(CheckHealth);
    }

    public void Init(TreeStatblock stats) {
        transform.localScale = stats.Size * transform.localScale;
        initialScaleY = transform.localScale.y;
        actionCooldown = stats.Cooldown;
        timeUntilNextAction = actionCooldown;
        timeUntilAutoDeath = gameSettings.treeLifetime;
        ability = gameObject.AddComponent<RangedAttackAbility>();
        ability.Init(stats, gameSettings);
        isInitialized = true;
    }

    private void Update() {
        if(GameLogic.PlayerRole is not PlayerRole.TOWER_DEFENSER || !isInitialized) {
            return;
        }

        timeUntilNextAction -= Time.deltaTime;
        if(timeUntilNextAction <= 0) {
            bool didAbility = ability.TryPerform();
            if(didAbility) {
                photonView.RPC(nameof(SetAnimationTriggerRPC), RpcTarget.All, "Attack");
                timeUntilNextAction = actionCooldown;
            }
        }

        // Auto death / lifetime
        timeUntilAutoDeath -= Time.deltaTime;
        if(timeUntilAutoDeath <= 0) {
            health.Damage(float.MaxValue);
        }
        else {
            float minScaleFactor = gameSettings.minTreeScaleFactorFromLifeLoss;
            float scaleFactor = minScaleFactor + (1f - minScaleFactor) * timeUntilAutoDeath / gameSettings.treeLifetime;

            Vector3 localScale = transform.localScale;
            localScale.y = scaleFactor * initialScaleY;
            transform.localScale = localScale;
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

    [PunRPC]
    private void SetAnimationTriggerRPC(string name) {
        animator.SetTrigger(name);
    }
}
