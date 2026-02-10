using UnityEngine;

[CreateAssetMenu(menuName = "Items/Guilt-Soaked Ledger")]
public class GuiltSoakedLedger : ItemData
{
    private bool subscribed = false;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        ApplyBase(stats);
        Subscribe(stats);
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        ApplyBase(stats);
        Subscribe(stats);
    }

    private void ApplyBase(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 1.5f;
            s.fireRate -= 0.05f;
            s.range += 1f;
        });
    }

    private void Subscribe(PlayerStats stats)
    {
        if (subscribed) return;
        subscribed = true;

     

        PlayerEvents.OnPlayerDamaged += () =>
        {
            stats.ModifyStat(s => s.damage -= 0.5f);
        };
    }
}
