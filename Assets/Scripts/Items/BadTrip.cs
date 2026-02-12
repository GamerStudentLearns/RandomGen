using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "Items/Bad Trip")]
public class BadTrip : ItemData
{
    private bool subscribed = false;

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.StartCoroutine(SafeStatChange(stats, s =>
        {
            s.damage += 0.5f;
            s.fireRate += 0.1f;
            s.shotSpeed -= 1f;
        }));
        
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.StartCoroutine(SafeStatChange(stats, s =>
        {
            s.damage += 0.5f;
            s.fireRate += 0.1f;
            s.shotSpeed -= 1f;
        }));
       
    }

    

    private IEnumerator SafeStatChange(PlayerStats stats, System.Action<PlayerStats> change)
    {
        yield return null;
        yield return null;
        stats.ModifyStat(change);
    }
}
