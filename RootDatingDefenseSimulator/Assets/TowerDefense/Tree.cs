using Photon.Pun;
using System;
using UnityEngine;

public class Tree : MonoBehaviour, IPunInstantiateMagicCallback {
    private Health health;
    private TreeAbility ability;

    private Action onDeath;

    private Character character;
    private float actionCooldown = 5f;
    private float timeUntilNextAction;

    private void Awake() {
        health = GetComponent<Health>();
        health.Init(1);
        health.SetHealthListener(CheckHealth);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        timeUntilNextAction = actionCooldown;
    }

    private void Update() {
        if(timeUntilNextAction <= 0) {
            ability.Perform();
            timeUntilNextAction = actionCooldown;
        }
    }

    public void AddDeathListener(Action deathListener) {
        onDeath += deathListener;
    }

    private void CheckHealth(float arg1, float arg2) {
        onDeath?.Invoke();
        PhotonNetwork.Destroy(gameObject);
    }
}
