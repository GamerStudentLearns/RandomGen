using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Forgotten Name")]
public class ForgottenName : ItemData
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

        RunEvents.OnFloorChanged += () =>
        {
            stats.StartCoroutine(SafeFloorChange(stats));
        };
    }

    private IEnumerator SafeFloorChange(PlayerStats stats)
    {
        yield return null;
        yield return null;

        ApplyRandom(stats, +1);
        ApplyRandom(stats, -1);
    }

    private void ApplyRandom(PlayerStats stats, int direction)
    {
        int roll = Random.Range(0, 4);

        stats.ModifyStat(s =>
        {
            switch (roll)
            {
                case 0: s.damage += 0.5f * direction; break;
                case 1: s.fireRate += 0.2f * direction; break;
                case 2: s.moveSpeed += 0.1f * direction; break;
                case 3: s.range += 1f * direction; break;
            }
        });
    }

    private IEnumerator SafeStatChange(PlayerStats stats, System.Action<PlayerStats> change)
    {
        yield return null;
        yield return null;
        stats.ModifyStat(change);
    }
}
