using UnityEngine;

[CreateAssetMenu(menuName = "Items/Small Rock")]
public class SmallRock : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 1f;
            s.moveSpeed -= 1f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 1f;
            s.moveSpeed -= 1f;
        });
    }
}
