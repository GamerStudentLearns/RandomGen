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

        // Correct: use PlayerHealth logic for heart containers
        PlayerHealth player = Object.FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.AddHeartContainer(1); // ❤️ correct behaviour
        }

        // Refresh UI
        RunManager.RunEvents.OnItemAcquired?.Invoke();
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
