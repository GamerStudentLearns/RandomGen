using UnityEngine;

[CreateAssetMenu(menuName = "Items/Staff Of Homa")]
public class StaffOfHoma : ItemData
{
    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        // Calculate bonus damage based on current red hearts
        float bonusDamage = run.currentHearts * 0.5f;

        stats.ModifyStat(s =>
        {
            s.damage += bonusDamage;
        });
    }
}

