using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public EnemyHealth bossHealth;
    public Image fill;

    void Update()
    {
        if (bossHealth == null) return;

        float percent = bossHealth.CurrentHealth / bossHealth.maxHealth;
        fill.fillAmount = percent;
    }
}
