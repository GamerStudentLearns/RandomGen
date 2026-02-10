using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Candle of Doubt")]
public class CandleOfDoubt : ItemData
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

        EnemySpawnEvents.OnEnemySpawned += (enemy) =>
        {
            if (enemy == null) return;
            var mod = enemy.AddComponent<EnemyDamageModifier>();
            mod.multiplier = 0.8f;
        };

        RoomEvents.OnRoomEntered += (room) =>
        {
            float roll = Random.Range(-0.1f, 0.1f);
            room.StartCoroutine(DelayedChange(stats, roll));
        };
    }

    private IEnumerator DelayedChange(PlayerStats stats, float roll)
    {
        yield return null;
        stats.ModifyStat(s => s.damage += roll * s.damage);
    }
}
