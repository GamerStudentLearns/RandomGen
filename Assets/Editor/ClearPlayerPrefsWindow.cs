using UnityEditor;
using UnityEngine;

public static class ClearPlayerPrefsWindow
{
    [MenuItem("Tools/Player Data/Clear PlayerPrefs")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared.");
    }
}
