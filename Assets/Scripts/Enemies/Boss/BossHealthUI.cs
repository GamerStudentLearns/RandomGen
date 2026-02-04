using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public static BossHealthUI instance;

    [SerializeField] private Slider slider;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    public void Show(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        gameObject.SetActive(true);
    }

    public void UpdateHealth(float current)
    {
        slider.value = current;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
