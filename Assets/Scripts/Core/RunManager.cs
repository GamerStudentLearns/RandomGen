using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

    [Header("Health")]
    public int baseMaxHearts = 6;
    public int heartModifiers = 0;   // Items add to this
    public int currentHearts;
    public int soulHearts;

    [Header("Run Data")]
    public List<ItemData> acquiredItems = new List<ItemData>();

    public int MaxHearts => baseMaxHearts + heartModifiers;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure valid defaults
        if (currentHearts <= 0)
            currentHearts = MaxHearts;

        if (soulHearts < 0)
            soulHearts = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset FIRST
        if (scene.name == "Level1")
        {
            ResetRun();
            HealPlayerOnStart();
        }

        // THEN sync PlayerHealth
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.maxHearts = MaxHearts;
            player.currentHearts = currentHearts;
            player.soulHearts = soulHearts;

            if (player.heartUI != null)
            {
                player.heartUI.Initialize(MaxHearts, soulHearts);
                player.heartUI.UpdateHearts(currentHearts, soulHearts);
            }
        }
    }


    public void ResetRun()
    {
        acquiredItems.Clear();

        // Reset modifiers
        heartModifiers = 0;

        // Reset hearts
        currentHearts = baseMaxHearts;
        soulHearts = 0;

        Debug.Log("Run reset — modifiers cleared, hearts restored, items cleared.");
    }

    private void HealPlayerOnStart()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player == null)
        {
            Debug.LogWarning("No PlayerHealth found in scene.");
            return;
        }

        // Force max hearts to 6 at the start of Level1
        player.maxHearts = 6;
        RunManager.instance.currentHearts = 6;

        // Heal the player to full (6)
        player.currentHearts = 6;

        // Sync soul hearts
        player.soulHearts = 0;
        RunManager.instance.soulHearts = 0;

        // Update UI
        if (player.heartUI != null)
        {
            player.heartUI.Initialize(6, 0);
            player.heartUI.UpdateHearts(6, 0);
        }

        Debug.Log("Player healed to 6 hearts on Level1 load.");
    }

}
