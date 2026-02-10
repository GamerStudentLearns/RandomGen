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

    [Header("Floor Tracking")]
    public int currentFloor = 1;

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
        if (scene.name == "Level1")
        {
            currentFloor = 1;
            ResetRun();
            HealPlayerOnStart();
        }
        else
        {
            currentFloor++;
            RunEvents.FloorChanged?.Invoke();
        }

        PlayerStats stats = FindFirstObjectByType<PlayerStats>();
        if (stats != null)
        {
            foreach (var item in acquiredItems)
                item.ApplyPersistent(stats, this);
        }

        // IMPORTANT: tell UI to refresh after items are reapplied
        RunEvents.OnItemAcquired?.Invoke();

        SyncPlayerHealthToRun();
    }

    public static class RunEvents
    {
        public static System.Action OnItemAcquired;
        public static System.Action FloorChanged;
    }

    public void AcquireItem(ItemData item, PlayerStats stats)
    {
        acquiredItems.Add(item);
        item.OnPickup(stats, this);

        RunEvents.OnItemAcquired?.Invoke();
    }

    public void ResetRun()
    {
        acquiredItems.Clear();

        heartModifiers = 0;
        currentHearts = baseMaxHearts;
        soulHearts = 0;
    }

    private void HealPlayerOnStart()
    {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        if (player == null)
            return;

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
