using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Candle of Doubt")]
public class CandleOfDoubt : ItemData
{
    private bool subscribed;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s => {
            s.fireRate += 0.5f;
            s.damage -= 0.5f;
            s.moveSpeed += 10f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s => {
            s.fireRate += 0.5f;
            s.damage -= 0.5f;
            s.moveSpeed += 10f;
        });
    }

}