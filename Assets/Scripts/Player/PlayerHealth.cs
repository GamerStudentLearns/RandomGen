using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 6;

    public GameObject deathEffect;


    public float invulnTime = 1f;

    [HideInInspector] public int currentHearts;
    private bool invulnerable;

    public string SceneToLoad;

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

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Disable or destroy the player
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneToLoad);

        // TODO: Trigger respawn or game over
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
