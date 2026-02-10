using UnityEngine;

[CreateAssetMenu(menuName = "Items/Vermin Crown")]
public class VerminCrown : ItemData
{
    private bool subscribed;
    public GameObject ratPrefab;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s => s.moveSpeed += 0.2f);
        Subscribe();
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s => s.moveSpeed += 0.2f);
        Subscribe();
    }

    private void Subscribe()
    {
        if (subscribed) return;
        subscribed = true;

        EnemyDeathEvents.OnEnemyDied += (enemy) =>
        {
            if (enemy == null) return;
            if (Random.value < 0.15f)
                Object.Instantiate(ratPrefab, enemy.transform.position, Quaternion.identity);
        };
    }
}
