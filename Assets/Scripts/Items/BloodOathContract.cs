using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Blood Oath Contract")]
public class BloodOathContract : ItemData
{
    

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 2f;
            s.fireRate -= 0.2f;
        });


    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 2f;
            s.fireRate -= 0.2f;
        });

    }
}




