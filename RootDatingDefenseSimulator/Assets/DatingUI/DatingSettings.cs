using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatingSettings", menuName = "ScriptableObjects/DatingSettings", order = 1)]
public class DatingSettings : ScriptableObject
{
    [System.Serializable]
    public class EmojiMatches
    {
        public string name;
        public Sprite[] emoji;
    }

    public int numChildrenPerDate = 1;
    public int numStartTrees = 4;
    public EmojiMatches[] matchingEmojis;
    public float randomizeNewTreeTimer = 10;
}
