using UnityEditor;
using UnityEngine;

public class SaveSlotDebugWindow : EditorWindow
{
    private int slot = 1;

    private bool level6Cleared;
    private bool specialUnlocked;

    [MenuItem("Tools/Save Slot Debugger")]
    public static void ShowWindow()
    {
        GetWindow<SaveSlotDebugWindow>("Save Slot Debugger");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save Slot Debug Controls", EditorStyles.boldLabel);

        // Slot selector
        slot = EditorGUILayout.IntSlider("Slot", slot, 1, 3);

        // Load current values
        if (GUILayout.Button("Load Slot Data"))
        {
            SaveSlotManager.CurrentSlot = slot;
            level6Cleared = SaveManager.HasClearedLevel6();
            specialUnlocked = SaveManager.HasUnlockedSpecialButton();
            Debug.Log($"Loaded Slot {slot}");
        }

        GUILayout.Space(10);

        // Editable toggles
        level6Cleared = EditorGUILayout.Toggle("Level 6 Cleared", level6Cleared);
        specialUnlocked = EditorGUILayout.Toggle("Special Button Unlocked", specialUnlocked);

        GUILayout.Space(10);

        // Apply changes
        if (GUILayout.Button("Apply Changes"))
        {
            SaveSlotManager.CurrentSlot = slot;

            PlayerPrefs.SetInt($"SaveSlot{slot}_Level6Cleared", level6Cleared ? 1 : 0);
            PlayerPrefs.SetInt($"SaveSlot{slot}_SpecialButtonUnlocked", specialUnlocked ? 1 : 0);

            PlayerPrefs.Save();
            Debug.Log($"Updated Slot {slot}");
        }

        GUILayout.Space(10);

        // Delete slot
        if (GUILayout.Button("Delete Slot"))
        {
            SaveManager.DeleteSlot(slot);
            Debug.Log($"Deleted Slot {slot}");
        }

        GUILayout.Space(10);

        // Refresh UI in scene
        if (GUILayout.Button("Refresh Menu UI"))
        {
            var menu = FindObjectOfType<MenuController>();
            if (menu != null)
            {
                menu.UpdateSlotStatusLabels();
                Debug.Log("Menu UI refreshed");
            }
            else
            {
                Debug.LogWarning("MenuController not found in scene");
            }
        }
    }
}
