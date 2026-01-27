using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

    public int currentHearts;
    public int maxHearts;
    public int soulHearts;

    public List<ItemData> acquiredItems = new List<ItemData>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // 🔥 IMPORTANT FIX — ensure valid defaults
        if (maxHearts <= 0)
            maxHearts = 6;

        if (currentHearts <= 0)
            currentHearts = maxHearts;

        if (soulHearts < 0)
            soulHearts = 0;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Restore saved health on every scene
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player != null)
        {
            player.maxHearts = maxHearts;
            player.currentHearts = currentHearts;
            player.soulHearts = soulHearts;

            if (player.heartUI != null)
            {
                player.heartUI.Initialize(maxHearts, soulHearts);
                player.heartUI.UpdateHearts(currentHearts, soulHearts);
            }
        }

        // Only reset run on Level1
        if (scene.name == "Level1")
        {
            ResetRun();
            HealPlayerOnStart();
        }
    }

    public void ResetRun()
    {
        acquiredItems.Clear();

        // Ensure valid values after reset
        if (maxHearts <= 0)
            maxHearts = 6;

        currentHearts = maxHearts;
        soulHearts = 0;

        Debug.Log("Run reset — hearts restored and item pool cleared.");
    }

    private void HealPlayerOnStart()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player == null)
        {
            Debug.LogWarning("No PlayerHealth found in scene.");
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            if (player.currentHearts >= player.maxHearts)
                break;

            player.Heal(1);
        }

        Debug.Log("Player healed for 6 on Level1 load.");
    }
}
