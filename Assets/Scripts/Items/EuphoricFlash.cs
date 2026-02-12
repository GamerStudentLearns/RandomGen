using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Euphoric Flash")]
public class EuphoricFlash : ItemData
{
    private bool subscribed = false;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range += 2f;
            s.fireRate -= 0.7f;
        });


    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
           s.range += 2f;
            s.fireRate -= 0.7f;
        });

    }
}
