using UnityEngine;

[CreateAssetMenu(menuName = "Items/Glowstick")]
public class Glowstick : ItemData
{
    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range += 2f;
        });
    }
}
