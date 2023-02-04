using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(DestructableObstacle))]
public class Tree : MonoBehaviour, IPunInstantiateMagicCallback {
    private Health health;
    private TreeAbility ability;
    [SerializeField]
    private GameSettings gameSettings;
    [SerializeField]
    private Animator animator;

    private Action onDeath;

    [SerializeField]
    private float actionCooldown = 5f;
    private float timeUntilNextAction;

    private void Awake() {
        Assert.IsNotNull(gameSettings);
        health = GetComponent<Health>();
        health.Init(gameSettings.treeHealth);
        health.AddHealthListener(CheckHealth);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        timeUntilNextAction = actionCooldown;
        ability = gameObject.AddComponent<RangedAttackAbility>();
        ability.Init(gameSettings, new Character());
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

    private void CheckHealth(float arg1, float arg2) {
        onDeath?.Invoke();
        // Destroyed in DestructableObstacle
    }
}
