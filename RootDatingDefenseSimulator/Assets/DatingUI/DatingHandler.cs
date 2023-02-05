using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class will keep the datable characters.
/// </summary>
public class DatingHandler : MonoBehaviour {
    [SerializeField] private HorizontalLayoutGroup availableTreeList;
    [SerializeField] private GameObject datingMain;

    [SerializeField] private TreeStatsSettings profileSettings;

    [SerializeField] private TreeButton[] treeButtons;
    [SerializeField] private TreeButton[] selectedProfiles;

    [SerializeField] private GameObject dateMinigameParent;
    [SerializeField] private DatingSpeechBubble datePromptBubble;
    [SerializeField] private DatingSpeechBubble[] dateAnswerBubble;
    private int correctAnswerBubbleIndex;

    [SerializeField] private bool DEBUG_OFFLINE_PLAY = false;

    public static TreeStatsSettings ProfileSettings { get; private set; }

    private TreeStatblock[] trees;
    private enum DateState { SelectTree, StartDatePrompt, Date };
    private DateState dateState;
    private int numCharacters;

    /// <summary>
    /// Debug purposes. Later characters will be shared.
    /// </summary>
    private void Awake() {
        ProfileSettings = profileSettings;

        if (DEBUG_OFFLINE_PLAY)
        {
            Debug.LogWarning("PLAYING AS OFFLINE PLAYER!");
        }
    }

    /// <summary>
    /// Debug class, later we'll read the profiles from a list of Characters 
    /// which will be shared between TD and DS.
    /// </summary>
    /// <param name="numCharacters"></param>
    public void Initialize() {
        datingMain.SetActive(true);
        numCharacters = treeButtons.Length;
        trees = new TreeStatblock[numCharacters];
        for(int i = 0; i < numCharacters; i++) {
            trees[i] = GenerateRandomTreeStats();
        }

        //Randomize initial list of trees. Later they'll be generated by dating.
        for(int i = 0; i < numCharacters; i++) {
            DisplayTreeProfile(trees[i], treeButtons[i]);
            treeButtons[i].index = i;

            //Apparently i is a pointer, so let's exorcise that little guy.
            int integerPointerExorcist = i;
            if(GameLogic.PlayerRole is PlayerRole.DATING_SIMULATOR ||
                DEBUG_OFFLINE_PLAY) {
                treeButtons[i].selectTreeButton.onClick.AddListener(delegate {
                    SelectTree(integerPointerExorcist);
                });
            }
            else if(GameLogic.PlayerRole is PlayerRole.TOWER_DEFENSER) {
                treeButtons[i].selectTreeButton.onClick.AddListener(delegate {
                    GameLogic.SetTreeSelected(integerPointerExorcist,
                        treeButtons[integerPointerExorcist]);

                });
            }
        }

        for(int i = 0; i < selectedProfiles.Length; i++) {
            selectedProfiles[i].gameObject.SetActive(false);

            //integerPointerExorcist explained in the for-loop above
            int integerPointerExorcist = i;
            selectedProfiles[i].selectTreeButton.onClick.AddListener(delegate { SelectedButtonDeselection(integerPointerExorcist); });
        }

        for(int i = 0; i < dateAnswerBubble.Length; i++) {
            //integerPointerExorcist explained in the for-loop above
            int integerPointerExorcist = i;
            dateAnswerBubble[i].button.onClick.AddListener(delegate { AnswerDatePrompt(integerPointerExorcist); });
        }

        dateState = DateState.SelectTree;
        dateMinigameParent.SetActive(false);
    }

    /// <summary>
    /// When clicking a tree they can be either selected or deselected.
    /// Selected trees are available for dating.
    /// </summary>
    /// <param name="tree"></param>
    public void SelectTree(int treeIndex) {
        //TODO: Unlock tree image. Also do this when TD player takes them from us.
        //If tree already is selected, deselect
        for(int i = 0; i < selectedProfiles.Length; i++) {
            if(!selectedProfiles[i].gameObject.activeInHierarchy)
                continue;
            if(selectedProfiles[i].index != treeIndex)
                continue;

            selectedProfiles[i].gameObject.SetActive(false);

            treeButtons[treeIndex].Highlight(false);
            //recover selected tree to list
            ShouldBeginDate();
            return;
        }

        //TODO: Lock tree image. Also do this when TD player takes them from us.
        //Add tree to selected
        for(int i = 0; i < selectedProfiles.Length; i++) {
            if(selectedProfiles[i].gameObject.activeInHierarchy)
                continue;

            DisplayTreeProfile(trees[treeIndex], selectedProfiles[i]);
            selectedProfiles[i].gameObject.SetActive(true);
            selectedProfiles[i].index = treeIndex;
            treeButtons[treeIndex].Highlight(true);

            break;
        }

        ShouldBeginDate();
    }

    public static TreeStatblock GenerateRandomTreeStats() {
        return new TreeStatblock(new List<int>() {
                    Random.Range(0, ProfileSettings.FaceSprites.Count),
                    Random.Range(0, ProfileSettings.TrunkSprites.Count),
                    Random.Range(0, ProfileSettings.Colors.Count),
                    Random.Range(0, ProfileSettings.Colors.Count),
                    Random.Range(0, ProfileSettings.PatternSprites.Count),
                    Random.Range(0, ProfileSettings.AttackTiers.Count),
                    Random.Range(0, ProfileSettings.CooldownTiers.Count),
                    Random.Range(0, ProfileSettings.RangeTiers.Count),
                    Random.Range(0, ProfileSettings.SizeTiers.Count),
                    Random.Range(0, ProfileSettings.Abilities.Count),
                });
    }

    private void DisplayTreeProfile(TreeStatblock tree, TreeButton button) {
        button.SetTree(tree);
    }

    private void ShouldBeginDate() {
        for(int i = 0; i < selectedProfiles.Length; i++) {
            if(!selectedProfiles[i].gameObject.activeInHierarchy) {
                return;
            }
        }

        BeginDate();
    }

    /// <summary>
    /// Selected trees can be returned from the new buttons which exist in
    /// the selected dating profiles.
    /// </summary>
    /// <param name="selectedProfileIndex"></param>
    public void SelectedButtonDeselection(int selectedProfileIndex) {
        if(dateState == DateState.Date)
            return;
        SelectTree(selectedProfiles[selectedProfileIndex].index);
    }

    /// <summary>
    /// Check if we can date, and if we can create 2 children to replace their parents with.
    /// </summary>
    public void TryDate() {
        if(!selectedProfiles[0].gameObject.activeInHierarchy)
            return;
        if(!selectedProfiles[1].gameObject.activeInHierarchy)
            return;

        //Date adults to make children.
        int tree1Index = selectedProfiles[0].index;
        int tree2Index = selectedProfiles[1].index;
        TreeStatblock child1 = DateQuoteOnQuote(trees[tree1Index], trees[tree2Index]);
        TreeStatblock child2 = DateQuoteOnQuote(trees[tree1Index], trees[tree2Index]);

        //Replace adults with children
        trees[tree1Index] = child1;
        trees[tree2Index] = child2;
        DisplayTreeProfile(child1, treeButtons[tree1Index]);
        DisplayTreeProfile(child2, treeButtons[tree2Index]);

        //Update dating profile pictures.
        EndDateAndReturnDaters();
    }

    public TreeStatblock DateQuoteOnQuote(TreeStatblock tree1, TreeStatblock tree2) {
        List<int> younglingStatsIndices = tree1.StatIndexes
            .Select((i, index) => {
                if(Random.value < 0.5f) {
                    return tree1.StatIndexes[index];
                }

                return tree2.StatIndexes[index];

            })
            .ToList();

        return new TreeStatblock(younglingStatsIndices);
    }

    public void BeginDate() {
        dateState = DateState.Date;
        dateMinigameParent.SetActive(true);
        //Todo set emoji prompt
        //Todo set emoji answers
        correctAnswerBubbleIndex = Random.Range(0, dateAnswerBubble.Length);
        for(int i = 0; i < dateAnswerBubble.Length; i++) {
            if(i == correctAnswerBubbleIndex) {
                dateAnswerBubble[i].emoji.color = new Color(1f, 0.6f, 0.5f);
                continue;
            }

            dateAnswerBubble[i].emoji.color = Color.white;
        }
    }

    private void AnswerDatePrompt(int answerIndex) {
        if(answerIndex == correctAnswerBubbleIndex) {
            TryDate();
        }
        else {
            //Update dating profile pictures.
            EndDateAndReturnDaters();
        }
    }

    private void EndDateAndReturnDaters() {
        int tree1Index = selectedProfiles[0].index;
        int tree2Index = selectedProfiles[1].index;
        treeButtons[tree1Index].Highlight(false);
        treeButtons[tree2Index].Highlight(false);
        treeButtons[tree1Index].gameObject.SetActive(true);
        treeButtons[tree2Index].gameObject.SetActive(true);
        selectedProfiles[0].gameObject.SetActive(false);
        selectedProfiles[1].gameObject.SetActive(false);

        dateState = DateState.SelectTree;
        dateMinigameParent.SetActive(false);
    }
}
