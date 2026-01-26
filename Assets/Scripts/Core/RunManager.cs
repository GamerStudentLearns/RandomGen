using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

    public int currentHearts;
    public int maxHearts;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            ResetRun();
            HealPlayerOnStart();
        }
    }

    public void ResetRun()
    {
        acquiredItems.Clear();
        currentHearts = maxHearts;
        Debug.Log("Run reset — hearts restored and item pool cleared.");
    }

    private void HealPlayerOnStart()
    {
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player == null)
        {
            Debug.LogWarning("No PlayerHealth found in scene.");
            return;
        }

        // Heal exactly 6 using the same logic as HeartPickup
        for (int i = 0; i < 6; i++)
        {
            if (player.currentHearts >= player.maxHearts)
                break;

            player.Heal(1);
        }

        Debug.Log("Player healed for 6 on Level1 load.");
    }
}
