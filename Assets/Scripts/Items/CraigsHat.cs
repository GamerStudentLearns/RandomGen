using UnityEngine;

[CreateAssetMenu(menuName = "Items/Craig'sHat")]
public class CraigHat : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 2f;
        });
    }
}
