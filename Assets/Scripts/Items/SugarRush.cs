using UnityEngine;

[CreateAssetMenu(menuName = "Items/Sugar Rush")]
public class SugarRush : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
    {
        // Stat changes
        stats.ModifyStat(s =>
        {
            s.shotSpeed += 3f;
            s.moveSpeed += 2f;
            s.fireRate -= 0.15f;
            s.damage -= 2f;
        });

        // Add soul heart
        run.soulHearts += 1;

        // Clamp current hearts
        run.currentHearts = Mathf.Min(run.currentHearts, run.MaxHearts);

        // Sync PlayerHealth + UI
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.soulHearts = run.soulHearts;
            player.currentHearts = run.currentHearts;

            if (player.heartUI != null)
            {
                player.heartUI.Initialize(player.maxHearts, player.soulHearts);
                player.heartUI.UpdateHearts(player.currentHearts, player.soulHearts);
            }
        }
    }
}
