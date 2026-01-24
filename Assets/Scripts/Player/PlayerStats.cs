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
        // Reapply all items the player has collected this run
        foreach (var item in RunManager.instance.acquiredItems)
        {
            item.Apply(this);
        }
    }


    public void ModifyStat(System.Action<PlayerStats>modifier)
    {
        modifier.Invoke(this);
        OnStatsChanged?.Invoke();
    }
}
