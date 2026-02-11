using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Blood-Oath Contract")]
public class BloodOathContract : ItemData
{
    private bool subscribed;
    private int enemiesThisRoom = 0;

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
            s.damage += 1f;
            s.fireRate -= 0.05f;
        });
    }

    private void Subscribe(PlayerStats stats)
    {
        if (subscribed) return;
        subscribed = true;

        RoomEvents.OnRoomEntered += (room) =>
        {
            enemiesThisRoom = 0;
            room.StartCoroutine(ApplyBonusNextFrame(stats));
        };

        EnemySpawnEvents.OnEnemySpawned += (_) =>
        {
            enemiesThisRoom++;
        };

        PlayerEvents.OnPlayerDamaged += () =>
        {
            stats.StartCoroutine(DelayedPenalty(stats));
        };
    }

    private IEnumerator ApplyBonusNextFrame(PlayerStats stats)
    {
        yield return null;

        if (enemiesThisRoom > 0)
        {
            float bonus = enemiesThisRoom * 0.25f;
            stats.ModifyStat(s => s.damage += bonus);
        }
    }

    private IEnumerator DelayedPenalty(PlayerStats stats)
    {
        yield return null;
        stats.ModifyStat(s => s.damage -= 0.5f);
    }
}
