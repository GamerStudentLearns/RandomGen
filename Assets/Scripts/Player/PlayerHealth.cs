using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 6;

    public GameObject deathEffect;
    public float invulnTime = 1f;

    [HideInInspector] public int currentHearts;
    private bool invulnerable;

    [Header("Soul Hearts")]
    public int soulHearts = 0;

    [Header("UI")]
    public HeartUI heartUI;

    [Header("Game Over UI")]
    public GameObject gameOverUI;

    private PlayerStats stats;

    private Renderer[] renderers;
    private Color[] originalColors;
    public float flashDuration = 0.1f;

    void Awake()
    {
        FindOrCreateHeartUI();

        if (RunManager.instance == null)
        {
            var rmGo = new GameObject("RunManager");
            rmGo.AddComponent<RunManager>();
        }

        maxHearts = RunManager.instance.MaxHearts;
        currentHearts = RunManager.instance.currentHearts;
        soulHearts = RunManager.instance.soulHearts;

        if (heartUI == null)
            heartUI = FindFirstObjectByType<HeartUI>();

        if (heartUI != null)
        {
            heartUI.Initialize(maxHearts, soulHearts);
            heartUI.UpdateHearts(currentHearts, soulHearts);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }
    }

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        PlayerEvents.PlayerStatsRef = stats;   // EVENT HOOK
    }

    public void TakeDamage(int dmg)
    {
        if (invulnerable) return;

        if (soulHearts > 0)
        {
            soulHearts -= dmg;

            if (soulHearts < 0)
            {
                currentHearts += soulHearts;
                soulHearts = 0;
            }
        }
        else
        {
            currentHearts -= dmg;
        }

        currentHearts = Mathf.Max(currentHearts, 0);

        RunManager.instance.currentHearts = currentHearts;
        RunManager.instance.soulHearts = soulHearts;

        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);

        HitStopController.instance.Stop(0.05f);

        StartCoroutine(FlashRed());

        StartCoroutine(Invulnerability());

        PlayerEvents.PlayerDamaged();   // EVENT HOOK

        if (currentHearts <= 0)
            Die();
    }

    public void AddSoulHearts(int amount)
    {
        soulHearts += amount;

        RunManager.instance.soulHearts = soulHearts;

        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);
    }

    IEnumerator Invulnerability()
    {
        invulnerable = true;
        yield return new WaitForSeconds(invulnTime);
        invulnerable = false;
    }

    private IEnumerator FlashRed()
    {
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];
    }

    private void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
        Time.timeScale = 0f;

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Heal(int amount)
    {
        currentHearts += amount;
        currentHearts = Mathf.Min(currentHearts, maxHearts);

        RunManager.instance.currentHearts = currentHearts;

        if (heartUI != null)
            heartUI.UpdateHearts(currentHearts, soulHearts);
    }

    private void FindOrCreateHeartUI()
    {
        if (heartUI != null)
            return;

        heartUI = FindFirstObjectByType<HeartUI>();

        if (heartUI == null)
        {
            GameObject prefab = Resources.Load<GameObject>("HeartUI");
            GameObject ui = Instantiate(prefab);
            heartUI = ui.GetComponent<HeartUI>();
        }
    }
}
