using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Euphoric Flash")]
public class EuphoricFlash : ItemData
{
    private bool subscribed = false;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        ApplyBase(stats);
        Subscribe();
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        ApplyBase(stats);
        Subscribe();
    }

    private void ApplyBase(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.fireRate += 0.3f;
            s.shotSpeed += 2f;
            s.damage += 1f;
        });
    }

    private void Subscribe()
    {
        if (subscribed) return;
        subscribed = true;

        RoomEvents.OnRoomEntered += (room) =>
        {
            PlayerStats stats = PlayerEvents.PlayerStatsRef;
            if (stats != null)
                room.StartCoroutine(DelayedStart(stats));
        };
    }

    private IEnumerator DelayedStart(PlayerStats stats)
    {
        // Wait one frame so UI exists
        yield return null;

        // Start the buff cycle
        stats.StartCoroutine(RoomBuff(stats));
    }

    private IEnumerator RoomBuff(PlayerStats stats)
    {
        // Euphoric high
        stats.ModifyStat(s =>
        {
            s.fireRate += 1.5f;
            s.damage += 2f;
        });

        yield return new WaitForSeconds(10f);

        // Crash
        stats.ModifyStat(s =>
        {
            s.fireRate -= 1.5f; // undo buff
            s.damage -= 2f;     // undo buff

            s.fireRate += 1f;   // crash penalty
            s.damage -= 1f;
        });

        yield return new WaitForSeconds(3f);

        // Recover from crash
        stats.ModifyStat(s =>
        {
            s.fireRate -= 1f;
            s.damage += 1f;
        });
    }
}
