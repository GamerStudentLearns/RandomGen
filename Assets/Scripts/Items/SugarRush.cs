using UnityEngine;

[CreateAssetMenu(menuName = "Items/Sugar Rush")]
public class SugarRush : ItemData
{
    public override void Apply(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 3f;
            s.moveSpeed += 2f;
            s.fireRate -= 0.15f;
            s.damage -= 2f;
            s.soulHeartsModifier += 1;
        });
    }
}
