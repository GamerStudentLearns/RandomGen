using UnityEngine;

[CreateAssetMenu(menuName = "Items/Overclocked Cortex")]
public class OverclockedCortex : ItemData
{
    private bool subscribed;

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
            s.fireRate -= 2f;
            s.shotSpeed -= 1f;
        });
    }

    private void Subscribe()
    {
        if (subscribed) return;
        subscribed = true;

        ProjectileEvents.OnPlayerProjectileFired += (proj) =>
        {
            if (proj == null) return;

            if (Random.value < 0.15f)
            {
                for (int i = 0; i < 2; i++)
                {
                    var clone = Object.Instantiate(proj.gameObject, proj.transform.position, Quaternion.identity);
                    clone.GetComponent<Projectile>().SetDirection(Random.insideUnitCircle.normalized);
                }
            }
        };
    }
}
