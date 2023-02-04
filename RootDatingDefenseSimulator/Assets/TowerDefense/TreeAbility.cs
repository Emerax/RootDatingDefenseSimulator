using UnityEngine;

[RequireComponent(typeof(Tree))]
public abstract class TreeAbility : MonoBehaviour {
    public abstract void Perform();
}
