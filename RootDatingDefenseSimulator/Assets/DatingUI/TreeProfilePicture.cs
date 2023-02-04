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
    [SerializeField] private GameObject highlightObject;
    public Button selectTreeButton;
    public int index; //Mostly used for external indexing.

    public void CopyTreeProfilePicture(TreeProfilePicture copyFrom)
    {
        faceImage.sprite = copyFrom.faceImage.sprite;
        //Don't copy index or selectTreeButton, they're for external use
    }

    public void Highlight(bool highlightOn)
    {
        highlightObject.SetActive(highlightOn);
    }
}
