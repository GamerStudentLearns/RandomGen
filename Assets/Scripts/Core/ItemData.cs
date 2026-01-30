using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;

    // Runs ONCE when the player picks up the item
    public virtual void OnPickup(PlayerStats stats, RunManager run) { }

    // Runs EVERY FLOOR (stat boosts, passives, etc.)
    public virtual void ApplyPersistent(PlayerStats stats, RunManager run) { }
}
