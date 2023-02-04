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
    public Image trunkImage;
    public Image background;
    public Image backgroundPattern;

    [SerializeField] private GameObject highlightObject;
    public Button selectTreeButton;
    public int index; //Mostly used for external indexing.

    public void Highlight(bool highlightOn)
    {
        highlightObject.SetActive(highlightOn);
    }
}
