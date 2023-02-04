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

    [SerializeField] private TreeProfilePicture[] characters;
    [SerializeField] private TreeProfilePicture[] selectedProfiles;
    [SerializeField] private GameObject datePrompt;

    /// <summary>
    /// Debug purposes. Later characters will be shared.
    /// </summary>
    private void Awake()
    {
        Initialize(8);
    }

    /// <summary>
    /// Debug class, later we'll read the profiles from a list of Characters 
    /// which will be shared between TD and DS.
    /// </summary>
    /// <param name="numCharacters"></param>
    public void Initialize(int numCharacters)
    {
        //Randomize initial list of trees. Later they'll be generated by dating.
        for(int i = 0; i < numCharacters; i++)
        {
            characters[i].faceImage.sprite = debugRandomizeFaces[Random.Range(0, debugRandomizeFaces.Length)];

            //Apparently i is a pointer, so let's exorcise that little guy.
            int integerPointerExorcist = i;
            characters[i].selectTreeButton.onClick.AddListener(delegate { SelectTree(integerPointerExorcist); });
        }

        for (int i = 0; i < selectedProfiles.Length; i++)
        {
            selectedProfiles[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// When clicking a tree they can be either selected or deselected.
    /// Selected trees are available for dating.
    /// </summary>
    /// <param name="tree"></param>
    public void SelectTree(int treeIndex)
    {
        TreeProfilePicture tree = characters[treeIndex];

        //If tree already is selected, deselect
        for(int i = 0; i < selectedProfiles.Length; i++)
        {
            if (!selectedProfiles[i].gameObject.activeInHierarchy)
                continue;
            if (selectedProfiles[i].index != treeIndex)
                continue;

            selectedProfiles[i].gameObject.SetActive(false);
            characters[treeIndex].Highlight(false);
            return;
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
            selectedProfiles[i].index = treeIndex;
            characters[treeIndex].Highlight(true);

            break;
        }
    }
}
