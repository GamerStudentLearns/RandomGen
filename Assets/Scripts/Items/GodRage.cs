using UnityEngine;

[CreateAssetMenu(menuName = "Items/GodRage")]
public class GodRage : ItemData
{
    // Runs ONCE when the player picks up the item
    public override void OnPickup(PlayerStats stats, RunManager run)
    {

        stats.ModifyStat(s =>
        {
            s.shotSpeed -= 1f;
            s.moveSpeed -= 1f;
            s.fireRate += 0.15f;
            s.damage -= 1f;
            s.range -= 1f;
        });

        // Add ONE soul heart on pickup
        run.soulHearts += 4;

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
