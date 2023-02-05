using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeStatsSettings", menuName = "ScriptableObjects/TreeStatsSettings", order = 1)]
public class TreeStatsSettings : ScriptableObject {
    [SerializeField]
    private List<Sprite> faceList = new();
    [SerializeField]
    private List<Sprite> trunkList = new();
    [SerializeField]
    private List<Color> colorList = new();
    [SerializeField]
    private List<Sprite> patternSpriteList = new();
    [SerializeField]
    private List<float> attackTiers = new();
    [SerializeField]
    private List<float> attackCooldownTiers = new();
    [SerializeField]
    private List<float> attackRangeTiers = new();
    [SerializeField]
    private List<float> sizeTiers = new();

    public float attackByGenerationMultiplier = 0.75f;
    public float sizeByGenerationMultiplier = 0.2f;

    private readonly List<Type> abilites = new() {
        typeof(RangedAttackAbility)
    };

    public System.Collections.ObjectModel.ReadOnlyCollection<Sprite> FaceSprites => faceList.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<Sprite> TrunkSprites => trunkList.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<Sprite> PatternSprites => patternSpriteList.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<Color> Colors => colorList.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<float> AttackTiers => attackTiers.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<float> CooldownTiers => attackCooldownTiers.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<float> RangeTiers => attackRangeTiers.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<float> SizeTiers => sizeTiers.AsReadOnly();
    public System.Collections.ObjectModel.ReadOnlyCollection<Type> Abilities => abilites.AsReadOnly();
}
