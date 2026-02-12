using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Guilt-Soaked Ledger")]
public class GuiltSoakedLedger : ItemData
{
    private bool subscribed;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        Subscribe(stats);
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        Subscribe(stats);
    }

    private void Subscribe(PlayerStats stats)
    {
        if (subscribed) return;
        subscribed = true;

        PlayerEvents.OnPlayerDamaged += () =>
        {
            if (stats != null)
                stats.StartCoroutine(SafeStatChange(stats, s =>
                {
                    // STRONGER, VISIBLE PENALTIES
                    s.damage -= 0.5f;
                    s.fireRate -= 0.15f;
                }));
        };
    }

    private IEnumerator SafeStatChange(PlayerStats stats, System.Action<PlayerStats> change)
    {
        // Wait TWO frames so StatDisplay is fully rebuilt
        yield return null;
        yield return null;

        stats.ModifyStat(change);
    }
}
