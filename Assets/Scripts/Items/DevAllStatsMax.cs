using UnityEngine;

[CreateAssetMenu(menuName = "Items/DevTest")]
public class DevAllStatsMax : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 300f;
            s.moveSpeed += 8f;
            s.fireRate -= 5f;
            s.range += 15f;
            s.shotSpeed += 20f;

        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 300f;
            s.moveSpeed += 8f;
            s.fireRate -= 5f;
            s.range += 15f;
            s.shotSpeed += 20f;
        });
    }
}
