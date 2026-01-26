using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;

    public abstract void Apply(PlayerStats stats);
}
