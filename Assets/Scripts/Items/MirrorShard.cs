using UnityEngine;

[CreateAssetMenu(menuName = "Items/Mirror Shard")]
public class MirrorShard : ItemData
{
    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 0.75f;
            s.fireRate += 0.05f;
            s.range += 1f;
            s.shotSpeed -= 2f;
        });
    }
}
