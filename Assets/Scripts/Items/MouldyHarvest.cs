using UnityEngine;

[CreateAssetMenu(menuName = "Items/Moldy Harvest")]
public class MoldyHarvest : ItemData
{
    private bool subscribed = false;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        ApplyStats(stats);
        Subscribe();
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        ApplyStats(stats);
        Subscribe();
    }

    private void ApplyStats(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 1.5f;
            s.shotSpeed -= 1f;
            s.range -= 1f;
        });
    }

    private void Subscribe()
    {
        if (subscribed) return;
        subscribed = true;

        ProjectileEvents.OnPlayerProjectileHitEnemy += (enemy) =>
        {
            enemy.StartCoroutine(ApplyPoison(enemy));
        };
    }

    private System.Collections.IEnumerator ApplyPoison(EnemyHealth enemy)
    {
        float duration = 3f;
        float tick = 1f;

        while (duration > 0)
        {
            if (enemy == null)
                yield break;

            enemy.TakeDamage(1f);   // guaranteed tick
            duration -= tick;

            yield return new WaitForSeconds(tick);
        }
    }

}
