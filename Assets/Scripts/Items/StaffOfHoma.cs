using UnityEngine;

[CreateAssetMenu(menuName = "Items/Staff Of Homa")]
public class StaffOfHoma : ItemData
{
    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 3f;
        });
      
    }

    public override void OnPickup(PlayerStats stats, RunManager run)
        {
            stats.ModifyStat(s =>
            {
                s.damage += 3f;
            });
    }
}

