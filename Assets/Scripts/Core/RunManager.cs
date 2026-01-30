using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

    [Header("Health")]
    public int baseMaxHearts = 6;
    public int heartModifiers = 0;
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

        if (currentHearts <= 0)
            currentHearts = MaxHearts;

        if (soulHearts < 0)
            soulHearts = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset run on first level
        if (scene.name == "Level1")
        {
            ResetRun();
            HealPlayerOnStart();
        }

        // Reapply persistent item effects
        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        if (stats != null)
        {
            foreach (var item in acquiredItems)
                item.ApplyPersistent(stats, this);
        }

        SyncPlayerHealthToRun();
    }

    // Called when the player picks up an item
    public void AcquireItem(ItemData item, PlayerStats stats)
    {
        acquiredItems.Add(item);
        item.OnPickup(stats, this);   // One-time effects
    }

    public void ResetRun()
    {
        acquiredItems.Clear();

        heartModifiers = 0;
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

        player.maxHearts = baseMaxHearts;
        currentHearts = baseMaxHearts;

        player.currentHearts = currentHearts;
        player.soulHearts = 0;
        soulHearts = 0;

        if (player.heartUI != null)
        {
            player.heartUI.Initialize(baseMaxHearts, 0);
            player.heartUI.UpdateHearts(baseMaxHearts, 0);
        }

        Debug.Log("Player healed to full on Level1 load.");
    }

    private void SyncPlayerHealthToRun()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player == null)
            return;

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
