using System.Collections;
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
    [SerializeField] private DatingSettings datingSettings;

    [SerializeField] private TreeStatsSettings profileSettings;

    [SerializeField] private TreeButton[] treeButtons;
    [SerializeField] private TreeButton[] selectedProfiles;

    [SerializeField] private GameObject dateMinigameParent;
    [SerializeField] private Animator dateFeedbackAnimator;
    [SerializeField] private DatingSpeechBubble datePromptBubble;
    [SerializeField] private DatingSpeechBubble[] dateAnswerBubble;
    private int correctAnswerBubbleIndex;

    [SerializeField] private bool DEBUG_OFFLINE_PLAY = false;

    public static TreeStatsSettings ProfileSettings { get; private set; }

    private TreeStatblock[] trees;
    private enum DateState { SelectTree, StartDatePrompt, Date };
    private DateState dateState;
    private int numCharacters;

    private float spawnNewTreeTimer;

    /// <summary>
    /// Debug purposes. Later characters will be shared.
    /// </summary>
    private void Awake() {
        ProfileSettings = profileSettings;

        if(DEBUG_OFFLINE_PLAY) {
            Debug.LogWarning("PLAYING AS OFFLINE PLAYER!");
        }

        if(GameLogic.PlayerRole is PlayerRole.DATING_SIMULATOR ||
                DEBUG_OFFLINE_PLAY)
        {
            spawnNewTreeTimer = datingSettings.randomizeNewTreeTimer;
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

        if(GameLogic.PlayerRole is PlayerRole.DATING_SIMULATOR || DEBUG_OFFLINE_PLAY) {
            for(int i = 0; i < numCharacters; i++) {
                if(i < datingSettings.numStartTrees)
                {
                    trees[i] = GenerateRandomTreeStats();
                }
                else
                {
                    trees[i] = null;
                }
            }
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
                    TowerDefensor.SetTreeSelected(treeButtons[integerPointerExorcist]);
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
            dateAnswerBubble[i].button.onClick.AddListener(delegate {
                StartCoroutine(AnswerDatePrompt(integerPointerExorcist));
            });
        }

        dateState = DateState.SelectTree;
        dateMinigameParent.SetActive(false);
    }

    private void Update()
    {
        if (GameLogic.PlayerRole is PlayerRole.DATING_SIMULATOR ||
                DEBUG_OFFLINE_PLAY)
        {
            spawnNewTreeTimer -= Time.deltaTime;
            if(spawnNewTreeTimer <= 0 && dateState != DateState.Date)
            {
                IntroduceNewSpecimenToGenePool();
                spawnNewTreeTimer = datingSettings.randomizeNewTreeTimer;
            }
        }
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

        StartCoroutine(DoDate());
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

        for (int i = 0; i < datingSettings.numChildrenPerDate; i++)
        {
            TreeStatblock child = DateQuoteOnQuote(trees[tree1Index], trees[tree2Index]);
            float avarageGeneration = (trees[tree1Index].generation + trees[tree2Index].generation) / 2.0f;
            child.generation = Mathf.FloorToInt(avarageGeneration) + 1;

            //Find open spot for child
            bool foundSpace = false;
            for (int spaceInd = 0; spaceInd < treeButtons.Length; spaceInd++)
            {
                if (trees[spaceInd] == null)
                {
                    trees[spaceInd] = child;
                    foundSpace = true;
                    DisplayTreeProfile(child, treeButtons[spaceInd]);
                    treeButtons[spaceInd].TriggerAnimation("NewTree");
                    break;
                }
            }

            if (!foundSpace)
            {
                if(Random.value < 0.5f)
                {
                    trees[tree1Index] = child;
                    DisplayTreeProfile(child, treeButtons[tree1Index]);
                    treeButtons[tree1Index].TriggerAnimation("NewTree");
                }
                else
                {
                    trees[tree2Index] = child;
                    DisplayTreeProfile(child, treeButtons[tree2Index]);
                    treeButtons[tree2Index].TriggerAnimation("NewTree");
                }
            }
        }

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

    IEnumerator DoDate() {
        dateState = DateState.Date;
        dateMinigameParent.SetActive(true);

        for(int i = 0; i < dateAnswerBubble.Length; i++) {
            dateAnswerBubble[i].gameObject.SetActive(false);
        }
        datePromptBubble.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.2f);
        selectedProfiles[0].TriggerAnimation("Talk");

        yield return new WaitForSeconds(1f);

        List<DatingSettings.EmojiMatches> availableEmojiMatches = datingSettings.matchingEmojis.ToList();

        //Choose correct emoji matchings and remove them from randomization list
        //so they can't be chosen again for wrong-answers
        int correctEmojiMatchIndex = Random.Range(0, availableEmojiMatches.Count);
        DatingSettings.EmojiMatches chosenMatch = availableEmojiMatches[correctEmojiMatchIndex];
        availableEmojiMatches.RemoveAt(correctEmojiMatchIndex);

        //Randomize one of the matches as prompt and one as the answer
        List<Sprite> emojiMatches = chosenMatch.emoji.ToList();
        int promptEmojiIndex = Random.Range(0, emojiMatches.Count);
        Sprite promptEmoji = emojiMatches[promptEmojiIndex];
        emojiMatches.RemoveAt(promptEmojiIndex);
        int correctAnswerEmojiIndex = Random.Range(0, emojiMatches.Count);
        Sprite correctAnswerEmoji = emojiMatches[correctAnswerEmojiIndex];
        emojiMatches.RemoveAt(correctAnswerEmojiIndex);

        //Set emojis in bubbles
        datePromptBubble.emoji.sprite = promptEmoji;
        datePromptBubble.gameObject.SetActive(true);
        datePromptBubble.animator.SetTrigger("Start");
        yield return new WaitForSeconds(0.8f);

        correctAnswerBubbleIndex = Random.Range(0, dateAnswerBubble.Length);
        for(int i = 0; i < dateAnswerBubble.Length; i++) {
            //Correct answer should match prompt emoji
            if(i == correctAnswerBubbleIndex) {
                dateAnswerBubble[i].emoji.sprite = correctAnswerEmoji;
            }
            else {
                //Otherwise pick an emoji from another random set
                int randEmojiMatchesIndex = Random.Range(0, availableEmojiMatches.Count);
                Sprite[] randomMatch = availableEmojiMatches[randEmojiMatchesIndex].emoji;
                availableEmojiMatches.RemoveAt(randEmojiMatchesIndex);
                int randomEmojiIndex = Random.Range(0, randomMatch.Length);
                Sprite randEmoji = randomMatch[randomEmojiIndex];

                dateAnswerBubble[i].emoji.sprite = randEmoji;
            }

            dateAnswerBubble[i].gameObject.SetActive(true);
            dateAnswerBubble[i].animator.SetTrigger("Start");
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator AnswerDatePrompt(int answerIndex) {
        for(int i = 0; i < dateAnswerBubble.Length; i++) {
            if(i == answerIndex)
                continue;
            dateAnswerBubble[i].gameObject.SetActive(false);
        }

        selectedProfiles[1].TriggerAnimation("Talk");
        yield return new WaitForSeconds(1.8f);
        selectedProfiles[0].TriggerAnimation("Talk");

        if (answerIndex == correctAnswerBubbleIndex)
        {
            dateFeedbackAnimator.SetTrigger("DateSuccess");
            yield return new WaitForSeconds(1f);
            TryDate();
        }
        else {
            //Update dating profile pictures.
            dateFeedbackAnimator.SetTrigger("DateFail");
            yield return new WaitForSeconds(1.2f);
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

    private void IntroduceNewSpecimenToGenePool()
    {
        for(int i = trees.Length-1; i >= 0; i--)
        {
            if (trees[i] != null)
                continue;
            trees[i] = GenerateRandomTreeStats();
            DisplayTreeProfile(trees[i], treeButtons[i]);
            treeButtons[i].TriggerAnimation("NewTreeOutsider");
            return;
        }
    }
}
