using UnityEngine;

[CreateAssetMenu(menuName = "Items/MouldyBanana")]
public class MouldyBanana : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
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
