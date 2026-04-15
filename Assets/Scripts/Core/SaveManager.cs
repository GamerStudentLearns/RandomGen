using UnityEngine;

public static class SaveManager
{
    private static string Key(string baseKey)
    {
        return $"SaveSlot{SaveSlotManager.CurrentSlot}_{baseKey}";
    }

    // -------------------------
    // LEVEL 6 CLEAR FLAG
    // -------------------------
    public static bool HasClearedLevel6()
    {
        return PlayerPrefs.GetInt(Key("Level6Cleared"), 0) == 1;
    }

    public static void SetLevel6Cleared()
    {
        PlayerPrefs.SetInt(Key("Level6Cleared"), 1);
        PlayerPrefs.Save();
    }

    // -------------------------
    // SPECIAL BUTTON UNLOCK FLAG
    // -------------------------
    public static bool HasUnlockedSpecialButton()
    {
        return PlayerPrefs.GetInt(Key("SpecialButtonUnlocked"), 0) == 1;
    }

    public static void SetSpecialButtonUnlocked()
    {
        PlayerPrefs.SetInt(Key("SpecialButtonUnlocked"), 1);
        PlayerPrefs.Save();
    }

    // -------------------------
    // DELETE SLOT
    // -------------------------
    public static void DeleteSlot(int slot)
    {
        PlayerPrefs.DeleteKey($"SaveSlot{slot}_Level6Cleared");
        PlayerPrefs.DeleteKey($"SaveSlot{slot}_SpecialButtonUnlocked");

        // Add more keys here later if you save more data per slot

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

}
