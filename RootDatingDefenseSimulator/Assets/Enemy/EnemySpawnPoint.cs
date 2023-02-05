using Photon.Pun;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawnPoint : MonoBehaviourPun {

    [SerializeField]
    private Animator animator;

    private void Awake() {
        Assert.IsNotNull(animator);
    }

    public void OnSpawn() {
        photonView.RPC(nameof(SetAnimationTriggerRPC), RpcTarget.All, "Spawn");
    }

    [PunRPC]
    private void SetAnimationTriggerRPC(string name) {
        animator.SetTrigger(name);
    }
}
