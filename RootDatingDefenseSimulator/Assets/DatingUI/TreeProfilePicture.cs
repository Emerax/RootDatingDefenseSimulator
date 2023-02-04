using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This will be the main object keeping the visual/dating-character, which is generated from the
/// shared character class.
/// </summary>
public class TreeProfilePicture : MonoBehaviour
{
    public Image faceImage;
    public Button selectTreeButton;
    private DatingHandler datingHandler;

    public void Initialize(DatingHandler datingHandler)
    {
        this.datingHandler = datingHandler;
        selectTreeButton.onClick.AddListener(SelectTree);
    }

    public void SelectTree()
    {
        Debug.Log("Clicked Button");
        datingHandler.SelectTree(this);
    }

    public void CopyTreeProfilePicture(TreeProfilePicture copyFrom)
    {
        faceImage.sprite = copyFrom.faceImage.sprite;
    }
}
