using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class will keep the datable characters.
/// </summary>
public class DatingHandler : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup availableTreeList;
    [SerializeField] private GameObject datingView;

    [SerializeField] private TreeProfilePicture treePrefab;
    [SerializeField] private Sprite[] debugRandomizeFaces;

    private List<TreeProfilePicture> characters = new List<TreeProfilePicture>();
    [SerializeField] private TreeProfilePicture[] selectedProfiles;

    /// <summary>
    /// Debug purposes. Later characters will be shared.
    /// </summary>
    private void Awake()
    {
        SetupRandom(8);
    }

    /// <summary>
    /// Debug class, later we'll read the profiles from a list of Characters 
    /// which will be shared between TD and DS.
    /// </summary>
    /// <param name="numCharacters"></param>
    public void SetupRandom(int numCharacters)
    {
        for(int i = 0; i < numCharacters; i++)
        {
            TreeProfilePicture newChar = Instantiate(treePrefab, availableTreeList.transform);
            newChar.faceImage.sprite = debugRandomizeFaces[Random.Range(0, debugRandomizeFaces.Length)];
            characters.Add(newChar);
            newChar.Initialize(this);
        }

        for (int i = 0; i < selectedProfiles.Length; i++)
        {
            if (selectedProfiles[i] == null)
                continue;

            selectedProfiles[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// When clicking a tree they can be either selected or deselected.
    /// Selected trees are available for dating.
    /// </summary>
    /// <param name="tree"></param>
    public void SelectTree(TreeProfilePicture tree)
    {
        //If tree already is selected, deselect
        for(int i = 0; i < selectedProfiles.Length; i++)
        {
            //if(selectedProfiles.)
        }

        //Add tree to selected
        for (int i = 0; i < selectedProfiles.Length; i++)
        {
            if (selectedProfiles[i] == null)
                continue;
            if (selectedProfiles[i].gameObject.activeInHierarchy)
                continue;

            selectedProfiles[i].CopyTreeProfilePicture(tree);
            selectedProfiles[i].gameObject.SetActive(true);
            return;
        }
    }
}
