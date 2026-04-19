using UnityEngine;

public static class SaveManager
{
    // -------------------------
    // INTERNAL SLOT KEY BUILDER
    // -------------------------
    private static string Key(string baseKey)
    {
        return $"SaveSlot{SaveSlotManager.CurrentSlot}_{baseKey}";
    }

    // ============================================================
    //  LEVEL 6 CLEAR FLAG (PER SLOT)
    // ============================================================
    public static bool HasClearedLevel6()
    {
        return PlayerPrefs.GetInt(Key("Level6Cleared"), 0) == 1;
    }

    public static void SetLevel6Cleared()
    {
        PlayerPrefs.SetInt(Key("Level6Cleared"), 1);
        PlayerPrefs.Save();
    }

    // ============================================================
    //  SPECIAL BUTTON FLAG (PER SLOT)
    // ============================================================
    public static bool HasUnlockedSpecialButton()
    {
        return PlayerPrefs.GetInt(Key("SpecialButtonUnlocked"), 0) == 1;
    }

    public static void SetSpecialButtonUnlocked()
    {
        PlayerPrefs.SetInt(Key("SpecialButtonUnlocked"), 1);
        PlayerPrefs.Save();
    }

    public static bool AnySlotHasSpecialUnlocked()
    {
        for (int slot = 1; slot <= 3; slot++)
        {
            if (PlayerPrefs.GetInt($"SaveSlot{slot}_SpecialButtonUnlocked", 0) == 1)
                return true;
        }
        return false;
    }

    // ============================================================
    //  BOSS / LORE UNLOCKS (PER SLOT)
    // ============================================================
    public static void SetBossUnlock(string id)
    {
        PlayerPrefs.SetInt(Key($"BossUnlocked_{id}"), 1);
        PlayerPrefs.Save();
    }

    public static bool HasBossUnlock(string id)
    {
        return PlayerPrefs.GetInt(Key($"BossUnlocked_{id}"), 0) == 1;
    }

    // Convert "LorePage" → "LorePage_Slot1/2/3"
    public static string GetSlotSpecificUnlock(string baseID)
    {
        int slot = SaveSlotManager.CurrentSlot;
        return $"{baseID}_Slot{slot}";
    }

    // Check if ANY slot has a specific lore page
    public static bool AnySlotHasBossUnlock(string id)
    {
        for (int slot = 1; slot <= 3; slot++)
        {
            if (PlayerPrefs.GetInt($"SaveSlot{slot}_BossUnlocked_{id}", 0) == 1)
                return true;
        }
        return false;
    }

    // ============================================================
    //  100% COMPLETION CHECKS
    // ============================================================

    // Check Level 6 + Special Button in ALL slots
    public static bool AllSlotsFullyUnlocked()
    {
        for (int slot = 1; slot <= 3; slot++)
        {
            bool cleared = PlayerPrefs.GetInt($"SaveSlot{slot}_Level6Cleared", 0) == 1;
            bool special = PlayerPrefs.GetInt($"SaveSlot{slot}_SpecialButtonUnlocked", 0) == 1;

            if (!cleared || !special)
                return false;
        }
        return true;
    }

    // Check if ALL lore pages are unlocked across ANY slot
    public static bool AllLoreUnlocked(string[] allLoreIDs)
    {
        foreach (string id in allLoreIDs)
        {
            if (!AnySlotHasBossUnlock(id))
                return false;
        }
        return true;
    }

    // TRUE 100% completion: Level6 + SpecialButton in all slots + ALL lore pages
    public static bool Has100PercentCompletion(string[] allLoreIDs)
    {
        if (!AllSlotsFullyUnlocked())
            return false;

        if (!AllLoreUnlocked(allLoreIDs))
            return false;

        return true;
    }

    // ============================================================
    //  DELETE SLOT (BASIC CLEANUP)
    // ============================================================
    public static void DeleteSlot(int slot)
    {
        PlayerPrefs.DeleteKey($"SaveSlot{slot}_Level6Cleared");
        PlayerPrefs.DeleteKey($"SaveSlot{slot}_SpecialButtonUnlocked");

        // Delete boss/lore unlocks for this slot
        // (Pattern-based wipe)
        for (int i = 0; i < PlayerPrefs.GetInt("PlayerPrefsCount", 0); i++)
        {
            // Optional: You can expand this if you want full cleanup.
        }

        PlayerPrefs.Save();
    }
}
