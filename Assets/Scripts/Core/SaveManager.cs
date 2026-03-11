using UnityEngine;

public static class SaveManager
{
    private static string Key(string baseKey)
    {
        return $"SaveSlot{SaveSlotManager.CurrentSlot}_{baseKey}";
    }

    public static bool HasClearedLevel6()
    {
        return PlayerPrefs.GetInt(Key("Level6Cleared"), 0) == 1;
    }

    public static void SetLevel6Cleared()
    {
        PlayerPrefs.SetInt(Key("Level6Cleared"), 1);
        PlayerPrefs.Save();
    }
    public static void DeleteSlot(int slot)
    {
        // Delete all keys for this slot
        PlayerPrefs.DeleteKey($"SaveSlot{slot}_Level6Cleared");

        // Add more keys here later if you save more data per slot

        PlayerPrefs.Save();
    }

}
