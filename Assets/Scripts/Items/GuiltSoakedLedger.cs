using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Guilt-Soaked Ledger")]
public class GuiltSoakedLedger : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s => 
        {
            s.shotSpeed -= 1f;
            s.damage += 0.5f;
            s.moveSpeed -= 0.2f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            
        });
    }
}
