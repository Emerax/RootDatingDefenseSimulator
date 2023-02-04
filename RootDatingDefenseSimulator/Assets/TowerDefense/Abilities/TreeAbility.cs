using UnityEngine;

[RequireComponent(typeof(Tree))]
public abstract class TreeAbility : MonoBehaviour {
    public abstract void Init(GameSettings gameSettings, Character character);
    public abstract bool TryPerform();
}
