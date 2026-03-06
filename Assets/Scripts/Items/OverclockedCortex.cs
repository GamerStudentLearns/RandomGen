using UnityEngine;

[CreateAssetMenu(menuName = "Items/Overclocked Cortex")]
public class OverclockedCortex : ItemData
{
    

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.fireRate += 1f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.fireRate += 1f;
        });
    }

    
}
