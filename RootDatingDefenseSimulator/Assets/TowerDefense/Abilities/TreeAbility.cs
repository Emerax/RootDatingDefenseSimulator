using UnityEngine;

[RequireComponent(typeof(TreeConscript))]
public abstract class TreeAbility : MonoBehaviour {
    public abstract void Init(TreeStatblock stats, GameSettings gameSettings);
    public abstract bool TryPerform();
}
