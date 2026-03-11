using UnityEngine;

public static class SaveManager
{
    private const string Level6ClearedKey = "Level6Cleared";

    // Returns true if the player has ever beaten Level 6
    public static bool HasClearedLevel6()
    {
        return PlayerPrefs.GetInt(Level6ClearedKey, 0) == 1;
    }

    // Call this once when the Level 6 boss dies
    public static void SetLevel6Cleared()
    {
        PlayerPrefs.SetInt(Level6ClearedKey, 1);
        PlayerPrefs.Save();
    }
}
