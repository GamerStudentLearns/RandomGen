using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 6;

    
    public float invulnTime = 1f;

    [HideInInspector] public int currentHearts;
    private bool invulnerable;

    [Header("UI")]
    public HeartUI heartUI;

    void Awake()
    {
        currentHearts = maxHearts;
        heartUI.Initialize(maxHearts);
        heartUI.UpdateHearts(currentHearts);
    }

    // --------------------
    // DAMAGE
    // --------------------
    public void TakeDamage(int dmg)
    {
        if (invulnerable) return;

        currentHearts -= dmg;
        currentHearts = Mathf.Max(currentHearts, 0);
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

    void Die()
    {
        Debug.Log("Player Dead");
        Destroy(gameObject);
        SceneManager.LoadScene("GameOverScene");
    }

    // --------------------
    // HEALING
    // --------------------
    public void Heal(int amount)
    {
        currentHearts += amount;
        if (currentHearts > maxHearts)
            currentHearts = maxHearts;

        heartUI.UpdateHearts(currentHearts);
        Debug.Log("Healed! Current hearts: " + currentHearts);
    }

}
