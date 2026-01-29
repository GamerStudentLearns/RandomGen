using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;

    // Items now receive both PlayerStats and RunManager
    public abstract void Apply(PlayerStats stats, RunManager run);
}
