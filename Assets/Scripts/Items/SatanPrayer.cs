using UnityEngine;

[CreateAssetMenu(menuName = "Items/Satan Prayer")]
public class SatanPrayer : ItemData
{
    public override void Apply(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 4f;
            s.maxHeartsModifier -= 3;
        });
    }
}
