using UnityEngine;

[CreateAssetMenu(menuName = "Items/Repent")]
public class Repent : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {

           
            s.damage += 10f;
            
        });

       
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range += 1f;
            s.shotSpeed += 1f;
            s.fireRate -= 0.1f;
            s.damage += 1f;
            s.moveSpeed += 1f;
        });
    }
}
