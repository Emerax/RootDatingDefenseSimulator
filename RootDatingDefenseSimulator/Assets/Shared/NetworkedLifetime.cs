using Photon.Pun;
using System.Collections;
using UnityEngine;

public class NetworkedLifetime : MonoBehaviourPun, IPunInstantiateMagicCallback {

    [SerializeField]
    private float lifetime;

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        if(GameLogic.PlayerRole == PlayerRole.TOWER_DEFENSER) {
            StartCoroutine(NetworkDestroyAfterSeconds(lifetime));
        }
    }

    private IEnumerator NetworkDestroyAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        PhotonNetwork.Destroy(photonView);
    }
}
