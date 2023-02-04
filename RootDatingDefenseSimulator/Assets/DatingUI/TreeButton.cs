using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This will be the main object keeping the visual/dating-character, which is generated from the
/// shared character class.
/// </summary>
public class TreeButton : MonoBehaviour {
    public Image faceImage;
    public Image trunkImage;
    public Image background;
    public Image backgroundPattern;

    [SerializeField] private GameObject highlightObject;
    public Button selectTreeButton;
    public int index; //Mostly used for external indexing.
    private TreeStatblock tree;
    private PhotonView photonView;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }

    public void Highlight(bool highlightOn) {
        highlightObject.SetActive(highlightOn);
    }

    public void UpdateProfile() {
        faceImage.sprite = tree.Face;
        trunkImage.sprite = tree.Trunk;
        background.color = tree.BackgroundCol;
        backgroundPattern.sprite = tree.BackgroundPattern;
        backgroundPattern.color = tree.PatternCol;
    }

    public void SetTree(TreeStatblock stats) {
        photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.AllBuffered, parameters: stats.StatIndexes.ToArray());
    }

    [PunRPC]
    private void SetTreeProfileRPC(int[] statIndices) {
        tree = new(statIndices.ToList());
        UpdateProfile();
    }
}
