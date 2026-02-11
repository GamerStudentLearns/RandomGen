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
                stats.StartCoroutine(DelayedPenalty(stats));
        };
    }

    private IEnumerator DelayedPenalty(PlayerStats stats)
    {
        // Wait one frame so StatDisplay is rebuilt
        yield return null;

        stats.ModifyStat(s =>
        {
            s.damage -= 0.2f;
            s.fireRate -= 0.1f;
        });
    }
}
