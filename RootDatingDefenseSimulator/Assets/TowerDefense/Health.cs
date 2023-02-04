using Photon.Pun;
using System;
using UnityEngine;

public class Health : MonoBehaviour {
    private PhotonView photonView;
    private float maxHealth;
    private float health;

    private Action<float> onTookDamage;
    private Action<float, float> onHealthChanged;

    public void Init(float maxHealth) {
        photonView = GetComponent<PhotonView>();
        this.maxHealth = maxHealth;
        health = maxHealth;
    }

    public void SetOnDamageListener(Action<float> damageListener) {
        onTookDamage += damageListener;
    }

    public void SetHealthListener(Action<float, float> healthListener) {
        onHealthChanged += healthListener;
    }

    public void Damage(float damage) {
        health -= damage;
        photonView.RPC(nameof(SetHealthRPC), RpcTarget.All, health);
        onTookDamage?.Invoke(damage);
    }

    private void SetHealthRPC(float newValue) {
        health = newValue;
        onHealthChanged?.Invoke(health, maxHealth);
    }
}
