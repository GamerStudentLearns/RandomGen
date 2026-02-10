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

        RoomEvents.OnRoomEntered += (_) =>
        {
            PlayerStats stats = PlayerEvents.PlayerStatsRef;
            stats.StartCoroutine(RoomBuff(stats));
        };
    }

    private IEnumerator RoomBuff(PlayerStats stats)
    {
        // Big buff
        stats.ModifyStat(s =>
        {
            s.fireRate += 1.5f;
            s.damage += 2f;
        });

        yield return new WaitForSeconds(10f);

        // Crash (temporary debuff)
        stats.ModifyStat(s =>
        {
            s.fireRate -= 1.5f; // fully undo fireRate buff
            s.damage -= 2f;     // fully undo damage buff
            s.fireRate += 1f;   // apply crash
            s.damage -= 1f;
        });

        yield return new WaitForSeconds(3f);

        // Undo crash
        stats.ModifyStat(s =>
        {
            s.fireRate -= 1f;
            s.damage += 1f;
        });
    }

}
