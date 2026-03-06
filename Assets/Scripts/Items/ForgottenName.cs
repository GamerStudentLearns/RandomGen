using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Forgotten Name")]
public class ForgottenName : ItemData
{
   

    public override void OnPickup(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range -= 0.5f;
            s.damage += 0.5f;
            s.fireRate -= 0.05f;
        });
    }

    public override void ApplyPersistent(PlayerStats stats, RunManager run)
    {
        stats.ModifyStat(s =>
        {
            s.range -= 0.5f;
            s.damage += 0.5f;
            s.fireRate -= 0.05f;
        });
    }

    
}
