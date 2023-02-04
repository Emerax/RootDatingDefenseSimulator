using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Saves the trees for dating as stat-blocks
/// </summary>
[System.Serializable]
public class TreeStatblock {
    readonly List<int> stats = new();

    public TreeStatblock(List<int> stats) {
        this.stats = stats;
    }

    public System.Collections.ObjectModel.ReadOnlyCollection<int> StatIndexes => stats.AsReadOnly();

    //Lover stats
    public Sprite Face => DatingHandler.ProfileSettings.FaceSprites[stats[0]];
    public Sprite Trunk => DatingHandler.ProfileSettings.TrunkSprites[stats[1]];
    public Color BackgroundCol => DatingHandler.ProfileSettings.Colors[stats[2]];
    public Color PatternCol => DatingHandler.ProfileSettings.Colors[stats[3]];
    public Sprite BackgroundPattern => DatingHandler.ProfileSettings.PatternSprites[stats[4]];

    //Fighter stats
    public float Attack => DatingHandler.ProfileSettings.AttackTiers[stats[5]];
    public float Cooldown => DatingHandler.ProfileSettings.CooldownTiers[stats[6]];
    public float Range => DatingHandler.ProfileSettings.RangeTiers[stats[7]];
    public float Size => DatingHandler.ProfileSettings.SizeTiers[stats[8]];
    public System.Type Ability => DatingHandler.ProfileSettings.Abilities[stats[9]];
}
