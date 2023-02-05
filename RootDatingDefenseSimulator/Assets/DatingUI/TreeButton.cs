using Photon.Pun;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This will be the main object keeping the visual/dating-character, which is generated from the
/// shared character class.
/// </summary>
public class TreeButton : MonoBehaviour
{
    public Image faceImage;
    public Image trunkImage;
    public Image background;
    public Image backgroundPattern;
    public TMP_Text generationLabel;

    [SerializeField] private GameObject highlightObject;
    [SerializeField] private Animator emoteAnimator;
    public Button selectTreeButton;
    public int index; //Mostly used for external indexing.

    public bool IsEmpty => tree == null;

    private TreeStatblock tree;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void Highlight(bool highlightOn)
    {
        highlightObject.SetActive(highlightOn);
        selectTreeButton.interactable = !highlightOn;
    }

    public bool GetHighlight()
    {
        return highlightObject.activeInHierarchy;
    }

    public void UpdateProfile()
    {
        if (tree == null)
        {
            faceImage.sprite = null;
            trunkImage.sprite = null;
            background.color = Color.gray;
            backgroundPattern.sprite = null;
            generationLabel.text = "";
        }
        else
        {
            faceImage.sprite = tree.Face;
            trunkImage.sprite = tree.Trunk;
            background.color = tree.BackgroundCol;
            backgroundPattern.sprite = tree.BackgroundPattern;
            backgroundPattern.color = tree.PatternCol;
            generationLabel.text = tree.generation.ToString();
        }
        selectTreeButton.interactable = tree != null;
    }

    public void SetTree(TreeStatblock stats)
    {
        if (stats == null)
        {
            photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.AllBuffered, new int[0], 0);
            return;
        }
        else
        {
            photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.AllBuffered, stats.StatIndexes.ToArray(), stats.generation);
        }

    }

    /// <summary>
    /// Returns the tree attached to this button and un-attaches it.
    /// The button is empty afterwards.
    public TreeStatblock PopTreeStats()
    {
        TreeStatblock treeturnValue = tree;
        photonView.RPC(nameof(SetTreeProfileRPC), RpcTarget.All, new int[0], 0);
        return treeturnValue;
    }

    public void TriggerAnimation(string triggerName)
    {
        emoteAnimator.SetTrigger(triggerName);
    }

    [PunRPC]
    private void SetTreeProfileRPC(int[] statIndices, int generation)
    {
        Debug.Log($"Tree Button {name} received {statIndices.Length} indices");
        if (statIndices.Length == 0)
        {
            tree = null;
        }
        else
        {
            tree = new(statIndices.ToList(), generation);
        }

        UpdateProfile();
    }
}
