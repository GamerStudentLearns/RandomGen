using UnityEngine;

[CreateAssetMenu(menuName = "Items/Meth")]
public class Meth : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range -= 3f;
            s.shotSpeed += 3f;
            s.fireRate -= 0.2f;
            s.damage -= 3f;
        });
    }
}
