using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Absorbent Sponge")]
public class AbsorbentSponge : ItemData
{
    private bool subscribed;
    private int absorbed;

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

        ProjectileEvents.OnEnemyProjectileDestroyed += () =>
        {
            absorbed++;
            if (absorbed >= 5)
            {
                absorbed = 0;
                stats.StartCoroutine(SafeStatChange(stats, s =>
                {
                    s.damage += 1f;
                }));
            }
        };

        RoomEvents.OnRoomEntered += (_) => absorbed = 0;
    }

    private IEnumerator SafeStatChange(PlayerStats stats, System.Action<PlayerStats> change)
    {
        yield return null;
        yield return null;
        stats.ModifyStat(change);
    }
}
