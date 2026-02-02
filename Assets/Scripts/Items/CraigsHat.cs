using UnityEngine;

[CreateAssetMenu(menuName = "Items/Craig'sHat")]
public class CraigHat : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 2f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 2f;
        });
    }
}
