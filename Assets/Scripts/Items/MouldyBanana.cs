using UnityEngine;

[CreateAssetMenu(menuName = "Items/MouldyBanana")]
public class MouldyBanana : ItemData
{
    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range += 1f;
            s.shotSpeed += 1f;
            s.fireRate -= 0.05f;
            s.damage += 1f;
            s.moveSpeed += 1f;
        });

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

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range += 1f;
            s.shotSpeed += 1f;
            s.fireRate -= 0.05f;
            s.damage += 1f;
            s.moveSpeed += 1f;
        });
    }
}
