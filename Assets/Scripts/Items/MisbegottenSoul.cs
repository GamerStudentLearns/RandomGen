using UnityEngine;

[CreateAssetMenu(menuName = "Items/Misbegotten Soul")]
public class MisbegottenSoul : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 3f;
            s.moveSpeed -= 7f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 3f;
            s.moveSpeed -= 7f;
        });
    }
}