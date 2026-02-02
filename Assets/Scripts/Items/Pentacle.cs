using UnityEngine;

[CreateAssetMenu(menuName = "Items/Pentacle")]
public class Pentacle : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {

      

        // One-time heart penalty
        run.heartModifiers += 1;

        // Clamp health
        int newMax = run.MaxHearts;
        run.currentHearts = Mathf.Clamp(run.currentHearts, 0, newMax);

        // Sync PlayerHealth
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

