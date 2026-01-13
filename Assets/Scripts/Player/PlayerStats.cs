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

    public void ModifyStat(System.Action<PlayerStats>modifier)
    {
        modifier.Invoke(this);
        OnStatsChanged?.Invoke();
    }
}
