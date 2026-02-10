using UnityEngine;

[CreateAssetMenu(menuName = "Items/Bad Trip")]
public class BadTrip : ItemData
{
    private bool subscribed = false;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        ApplyStats(stats);
        Subscribe(stats);
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        ApplyStats(stats);
        Subscribe(stats);
    }

    private void ApplyStats(PlayerStats stats)
    {
        stats.ModifyStat(s =>
        {
            s.damage += 0.5f;
            s.fireRate += 0.1f;
            s.shotSpeed -= 1f;
        });
    }

    private void Subscribe(PlayerStats stats)
    {
        if (subscribed) return;
        subscribed = true;

        RoomEvents.OnRoomEntered += (_) =>
        {
            float roll = Random.value;

            stats.ModifyStat(s =>
            {
                if (roll < 0.33f) s.damage += 0.5f;
                else if (roll < 0.66f) s.fireRate += 0.1f;
                else s.shotSpeed += 1f;
            });
        };

        
    }
}
