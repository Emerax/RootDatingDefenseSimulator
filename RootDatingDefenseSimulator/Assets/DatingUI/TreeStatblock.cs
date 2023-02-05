using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Saves the trees for dating as stat-blocks
/// </summary>
[System.Serializable]
public class TreeStatblock {
    readonly List<int> stats = new();

    public TreeStatblock(List<int> stats, int generation = 0) {
        this.stats = stats;
        this.generation = generation;
    }

    public int generation;

    public System.Collections.ObjectModel.ReadOnlyCollection<int> StatIndexes => stats.AsReadOnly();

    //Lover stats
    public Sprite Face => DatingHandler.ProfileSettings.FaceSprites[stats[0]];
    public Sprite Trunk => DatingHandler.ProfileSettings.TrunkSprites[stats[1]];
    public Color BackgroundCol => DatingHandler.ProfileSettings.Colors[stats[2]];
    public Color PatternCol => DatingHandler.ProfileSettings.Colors[stats[3]];
    public Sprite BackgroundPattern => DatingHandler.ProfileSettings.PatternSprites[stats[4]];

    //Fighter stats
    public float Attack => DatingHandler.ProfileSettings.AttackTiers[stats[0]] +
        generation * DatingHandler.ProfileSettings.attackByGenerationMultiplier;
    public float Cooldown => DatingHandler.ProfileSettings.CooldownTiers[stats[6]];
    public float Range => DatingHandler.ProfileSettings.RangeTiers[stats[2]];
    public float Size => DatingHandler.ProfileSettings.SizeTiers[stats[4]] +
        generation * DatingHandler.ProfileSettings.sizeByGenerationMultiplier;
    public System.Type Ability => DatingHandler.ProfileSettings.Abilities[stats[9]];
}
