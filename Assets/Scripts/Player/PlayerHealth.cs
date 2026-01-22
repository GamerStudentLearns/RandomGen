using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 6;

    public GameObject deathEffect;
    public float invulnTime = 1f;

    [HideInInspector] public int currentHearts;
    private bool invulnerable;

    [Header("UI")]
    public HeartUI heartUI;

    [Header("Game Over UI")]
    public GameObject gameOverUI;   // Assign in Inspector

    void Awake()
    {
        // If this is the first scene, initialize run data
        if (RunManager.instance.currentHearts == 0)
        {
            RunManager.instance.maxHearts = maxHearts;
            RunManager.instance.currentHearts = maxHearts;
        }

        // Apply stored health
        maxHearts = RunManager.instance.maxHearts;
        currentHearts = RunManager.instance.currentHearts;

        heartUI.Initialize(maxHearts);
        heartUI.UpdateHearts(currentHearts);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }


    // --------------------
    // DAMAGE
    // --------------------
    public void TakeDamage(int dmg)
    {
        if (invulnerable) return;

        currentHearts -= dmg;
        currentHearts = Mathf.Max(currentHearts, 0);

        RunManager.instance.currentHearts = currentHearts;

        heartUI.UpdateHearts(currentHearts);

        HitStopController.instance.Stop(0.05f);
        StartCoroutine(Invulnerability());

        if (currentHearts <= 0)
            Die();
    }

   


    IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
    }

    private void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // Destroy player object (replaces SetActive(false))
        Destroy(gameObject);

        // Freeze gameplay (same behavior as PauseMenu.Pause)
        Time.timeScale = 0f;

        // Show Game Over UI
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Show mouse cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // --------------------
    // HEALING
    // --------------------
    public void Heal(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Min(currentHearts, maxHearts);

        RunManager.instance.currentHearts = currentHearts;

        heartUI.UpdateHearts(currentHearts);
    }
}
