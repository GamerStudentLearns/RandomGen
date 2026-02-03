using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public Slider slider;
    private EnemyHealth bossHealth;

    void Start()
    {
        gameObject.SetActive(false); // hidden until a boss appears
    }

    public void SetBoss(EnemyHealth health)
    {
        bossHealth = health;
        slider.maxValue = health.maxHealth;
        slider.value = health.maxHealth;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (bossHealth == null)
        {
            gameObject.SetActive(false);
            return;
        }

        slider.value = bossHealth.CurrentHealth;
    }
}
