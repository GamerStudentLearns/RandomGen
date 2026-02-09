using UnityEngine;

[CreateAssetMenu(menuName = "Items/Bella Muerte Key")]
public class BellaMuerteKey : ItemData
{
    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 3f;
            s.range += 2f;
        });

    }

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 3f;
            s.range += 2f;
        });
    }
}

