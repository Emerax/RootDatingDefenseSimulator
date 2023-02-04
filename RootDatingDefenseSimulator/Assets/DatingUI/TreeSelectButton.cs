using Photon.Pun;
using System.Linq;
using UnityEngine;

public class TreeSelectButton : MonoBehaviour {
    private PhotonView photonView;
    private TreeProfilePicture treePicture;
    private TreeStatblock treeStats;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
        treePicture = GetComponent<TreeProfilePicture>();
    }

    public void SetTree(TreeStatblock stats) {
        photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.All, parameters: stats.StatIndexes);
    }

    [PunRPC]
    private void SetTreeProfileRPC(int[] statIndices) {
        treeStats = new(statIndices.ToList());
    }
}
