using UnityEngine;

[CreateAssetMenu(menuName = "Items/Satan Prayer")]
public class SatanPrayer : ItemData
{
    public override void Apply(PlayerStats stats, RunManager run)
    {
        // Stat changes
        stats.ModifyStat(s =>
        {
            s.damage += 4f;
        });

        // Remove 3 red heart containers
        run.heartModifiers -= 3;

        // Update max hearts
        int newMax = run.MaxHearts;
        run.currentHearts = Mathf.Clamp(run.currentHearts, 0, newMax);

        // Sync PlayerHealth + UI
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.maxHearts = newMax;
            player.currentHearts = run.currentHearts;

            if (player.heartUI != null)
            {
                player.heartUI.Initialize(newMax, player.soulHearts);
                player.heartUI.UpdateHearts(player.currentHearts, player.soulHearts);
            }
        }
    }
}
