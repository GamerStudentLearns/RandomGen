using UnityEngine;

public class FighterHitbox : MonoBehaviour
{
    private EnemyHealth parentHealth;

    private void Start()
    {
        parentHealth = GetComponentInParent<EnemyHealth>();
    }

    public void TakeDamage(float dmg)
    {
        parentHealth.TakeDamage(dmg);
    }
}
