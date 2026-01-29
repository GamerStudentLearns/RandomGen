using UnityEngine;

[CreateAssetMenu(menuName = "Items/Small Rock")]
public class SmallRock : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 1f;
            s.moveSpeed -= 1f;
        });
    }
}
