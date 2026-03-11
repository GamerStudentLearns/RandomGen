using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Absorbent Sponge")]
public class AbsorbentSponge : ItemData
{
    

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 1.5f;
            s.fireRate -= 0.1f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 1.5f;
            s.fireRate -= 0.1f;
        });
    }

    
}
