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
            var mod = enemy.gameObject.AddComponent<EnemyDamageModifier>();
            mod.multiplier = 0.8f;
        };

        RoomEvents.OnRoomEntered += (room) =>
        {
            float roll = Random.Range(-0.1f, 0.1f);
            stats.StartCoroutine(SafeStatChange(stats, s =>
            {
                s.damage += roll * s.damage;
            }));
        };
    }

    private IEnumerator SafeStatChange(PlayerStats stats, System.Action<PlayerStats> change)
    {
        yield return null;
        yield return null;
        stats.ModifyStat(change);
    }
}
