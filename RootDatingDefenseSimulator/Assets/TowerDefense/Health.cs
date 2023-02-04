using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Health : MonoBehaviour {

    public enum HealthPreset {
        None = 0,
        FunGoernForest = 1,
        Rock = 2,
    };

    [SerializeField]
    private HealthPreset useHealthPreset = HealthPreset.None;

    [Header("Only used if using Health Preset other than None")]
    [SerializeField]
    private GameSettings gameSettings;

    private PhotonView photonView;
    private float maxHealth;
    private float health;

    private Action<float> onTookDamage;
    private Action<float, float> onHealthChanged;

    private void Awake() {
        if(useHealthPreset != HealthPreset.None) {
            Assert.IsNotNull(gameSettings);
            switch(useHealthPreset) {
                case HealthPreset.FunGoernForest:
                    Init(gameSettings.funGoernForestHealth);
                    break;
                case HealthPreset.Rock:
                    Init(gameSettings.rockHealth);
                    break;
                default:
                    throw new Exception($"Health preset {useHealthPreset} support not implemented.");
            }
        }
    }

    public void Init(float maxHealth) {
        photonView = GetComponent<PhotonView>();
        this.maxHealth = maxHealth;
        health = maxHealth;
    }

    public void SetOnDamageListener(Action<float> damageListener) {
        onTookDamage += damageListener;
    }

    public void AddHealthListener(Action<float, float> healthListener) {
        onHealthChanged += healthListener;
    }

    public void Damage(float damage) {
        health -= damage;
        photonView.RPC(nameof(SetHealthRPC), RpcTarget.All, health);
        onTookDamage?.Invoke(damage);
    }

    [PunRPC]
    private void SetHealthRPC(float newValue) {
        health = newValue;
        onHealthChanged?.Invoke(health, maxHealth);
    }
}
