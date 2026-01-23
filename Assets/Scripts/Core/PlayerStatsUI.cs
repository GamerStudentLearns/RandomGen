using UnityEngine;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public TextMeshProUGUI statsText;

    private void OnEnable()
    {
        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerStats>();

        playerStats.OnStatsChanged += UpdateStatsUI;
        UpdateStatsUI();
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.OnStatsChanged -= UpdateStatsUI;
    }

    private void UpdateStatsUI()
    {
        statsText.text =
            $"Damage: {playerStats.damage}\n" +
            $"Fire Rate: {playerStats.fireRate}\n" +
            $"Move Speed: {playerStats.moveSpeed}\n" +
            $"Range: {playerStats.range}\n" +
            $"Shot Speed: {playerStats.shotSpeed}";
    }
}
