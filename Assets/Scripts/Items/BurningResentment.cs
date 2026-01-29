using UnityEngine;

[CreateAssetMenu(menuName = "Items/Burning Resentment")]
public class BurningResentment : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 2f;
            s.fireRate -= 0.1f;
            s.shotSpeed += 4f;
            s.range -= 2f;
        });
    }
}
