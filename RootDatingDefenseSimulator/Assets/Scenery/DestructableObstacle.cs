using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestructableObstacle : MonoBehaviourPun {

    private Health health;

    private void Awake() {
        health = GetComponent<Health>();
        health.AddHealthListener(CheckDeath);
        Enemy.RegisterDestructableObstacle(health);
    }

    private void CheckDeath(float health, float maxHealth) {
        if(health <= 0) {
            PhotonNetwork.Destroy(photonView);
        }
    }

    private void OnDestroy() {
        Enemy.DeregisterDestructableObstacle(health);
    }
}
