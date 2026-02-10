using UnityEngine;
using TMPro;
using System.Collections;

public class StatDisplay : MonoBehaviour
{
    public PlayerStats stats;

    public TMP_Text damageText;
    public TMP_Text fireRateText;
    public TMP_Text speedText;
    public TMP_Text rangeText;
    public TMP_Text shotSpeedText;

    private float lastDamage;
    private float lastFireRate;
    private float lastSpeed;
    private float lastRange;
    private float lastShotSpeed;

    private Color normalColor = Color.white;
    private float flashDuration = 1.2f;

    void Start()
    {
        if (stats == null)
            stats = FindFirstObjectByType<PlayerStats>();

        CacheStats();
        stats.OnStatsChanged += UpdateUI;
        UpdateUI();
    }

    void CacheStats()
    {
        lastDamage = stats.damage;
        lastFireRate = stats.fireRate;
        lastSpeed = stats.moveSpeed;
        lastRange = stats.range;
        lastShotSpeed = stats.shotSpeed;
    }

    void UpdateUI()
    {
        CheckChange(lastDamage, stats.damage, damageText, "DMG");
        CheckChange(lastFireRate, stats.fireRate, fireRateText, "Tears", invert: true);
        CheckChange(lastSpeed, stats.moveSpeed, speedText, "Speed");
        CheckChange(lastRange, stats.range, rangeText, "Range");
        CheckChange(lastShotSpeed, stats.shotSpeed, shotSpeedText, "Shot Spd");

        CacheStats();
    }

    void CheckChange(float oldVal, float newVal, TMP_Text text, string label, bool invert = false)
    {
        text.text = $"{label}: {newVal:F2}";

        float diff = newVal - oldVal;
        if (Mathf.Abs(diff) < 0.001f)
            return;

        bool wentUp = diff > 0;

        if (invert)
            wentUp = !wentUp;

        Color flashColor = wentUp ? Color.green : Color.red;
        StartCoroutine(FlashColor(text, flashColor));
    }

    IEnumerator FlashColor(TMP_Text text, Color flashColor)
    {
        text.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        text.color = normalColor;
    }
}
