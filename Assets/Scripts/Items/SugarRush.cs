using UnityEngine;

[CreateAssetMenu(menuName = "Items/Sugar Rush")]
public class SugarRush : ItemData
{
    public override void Apply(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 3f;
            s.moveSpeed += 1.5f;
            s.fireRate -= 0.25f;
            s.damage -= 2f;
        });
    }
}
