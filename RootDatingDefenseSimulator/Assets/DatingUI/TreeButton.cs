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
    [SerializeField] private Animator emoteAnimator;
    public Button selectTreeButton;
    public int index; //Mostly used for external indexing.

    public bool IsEmpty => tree == null;

    private TreeStatblock tree;
    private PhotonView photonView;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }

    public void Highlight(bool highlightOn) {
        highlightObject.SetActive(highlightOn);
        selectTreeButton.interactable = !highlightOn;
    }

    public bool GetHighlight() {
        return highlightObject.activeInHierarchy;
    }

    public void UpdateProfile() {
        if(tree == null) {
            faceImage.sprite = null;
            trunkImage.sprite = null;
            background.color = Color.gray;
            backgroundPattern.sprite = null;
        }
        else {
            faceImage.sprite = tree.Face;
            trunkImage.sprite = tree.Trunk;
            background.color = tree.BackgroundCol;
            backgroundPattern.sprite = tree.BackgroundPattern;
            backgroundPattern.color = tree.PatternCol;
        }
    }

    public void SetTree(TreeStatblock stats) {
        if(stats == null) {
            photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.AllBuffered, parameters: new int[0]);
            return;
        }
        photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.AllBuffered, parameters: stats.StatIndexes.ToArray());
    }

    /// <summary>
    /// Returns the tree attached to this button and un-attaches it.
    /// The button is empty afterwards.
    public TreeStatblock PopTreeStats() {
        TreeStatblock treeturnValue = tree;
        tree = null;
        UpdateProfile();
        return treeturnValue;
    }

    public void TriggerAnimation(string triggerName) {
        emoteAnimator.SetTrigger(triggerName);
    }

    [PunRPC]
    private void SetTreeProfileRPC(int[] statIndices) {
        if(statIndices.Length == 0) {
            tree = null;
        }
        else {
            tree = new(statIndices.ToList());
        }

        UpdateProfile();
    }
}
