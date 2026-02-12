using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float damage = 3.5f;
    public float fireRate = 0.3f;
    public float moveSpeed = 5f;
    public float range = 8f;
    public float shotSpeed = 10f;

    // Min/Max Values
    private const float MIN_DAMAGE = 0.1f;
    private const float MAX_DAMAGE = 300f;

    private const float MIN_SPEED = 0.3f;
    private const float MAX_SPEED = 15f;

    private const float MIN_FIRERATE = 0.01f;
    private const float MAX_FIRERATE = 5f;

    private const float MIN_RANGE = 2f;
    private const float MAX_RANGE = 15f;

    private const float MIN_SHOTSPEED = 3f;
    private const float MAX_SHOTSPEED = 20f;

    public delegate void StatsChanged();
    public event StatsChanged OnStatsChanged;

    // No Start() needed anymore

    public void ModifyStat(System.Action<PlayerStats> modifier)
    {
        modifier.Invoke(this);
        ClampAllStats();
        OnStatsChanged?.Invoke();
    }

    private void ClampAllStats()
    {
        damage = Mathf.Clamp(damage, MIN_DAMAGE, MAX_DAMAGE);
        moveSpeed = Mathf.Clamp(moveSpeed, MIN_SPEED, MAX_SPEED);
        fireRate = Mathf.Clamp(fireRate, MIN_FIRERATE, MAX_FIRERATE);
        range = Mathf.Clamp(range, MIN_RANGE, MAX_RANGE);
        shotSpeed = Mathf.Clamp(shotSpeed, MIN_SHOTSPEED, MAX_SHOTSPEED);
    }
}
