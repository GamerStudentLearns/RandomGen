using UnityEngine;

[CreateAssetMenu(menuName = "Items/Glowstick")]
public class Glowstick : ItemData
{
    public override void Apply(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.range += 2f;
        });
    }
}
