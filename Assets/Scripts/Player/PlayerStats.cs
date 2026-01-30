using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float damage = 3.5f;
    public float fireRate = 0.3f;
    public float moveSpeed = 5f;
    public float range = 8f;
    public float shotSpeed = 10f;

    public delegate void StatsChanged();
    public event StatsChanged OnStatsChanged;

    private void Start()
    {
        RunManager run = RunManager.instance;

        // Reapply persistent effects on scene load
        foreach (var item in run.acquiredItems)
            item.ApplyPersistent(this, run);
    }

    public void ModifyStat(System.Action<PlayerStats> modifier)
    {
        modifier.Invoke(this);
        OnStatsChanged?.Invoke();
    }
}
