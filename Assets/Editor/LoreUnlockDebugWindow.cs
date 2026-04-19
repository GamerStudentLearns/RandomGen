using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LoreUnlockDebugWindow : EditorWindow
{
    private BossLogMenu bossLogMenu;
    private List<string> loreIDs = new List<string>();
    private int selectedIndex = 0;

    [MenuItem("Tools/Lore Debug/Lore Unlocker")]
    public static void ShowWindow()
    {
        GetWindow<LoreUnlockDebugWindow>("Lore Unlocker");
    }

    private void OnFocus()
    {
        RefreshLoreList();
    }

    private void OnGUI()
    {
        GUILayout.Label("Lore Page Unlocker", EditorStyles.boldLabel);

        if (bossLogMenu == null)
        {
            EditorGUILayout.HelpBox("No BossLogMenu found in the scene.", MessageType.Warning);

            if (GUILayout.Button("Refresh"))
                RefreshLoreList();

            return;
        }

        if (loreIDs.Count == 0)
        {
            EditorGUILayout.HelpBox("BossLogMenu has no lore entries.", MessageType.Warning);
            return;
        }

        // Dropdown
        selectedIndex = EditorGUILayout.Popup("Lore Page", selectedIndex, loreIDs.ToArray());

        GUILayout.Space(10);

        // Unlock selected
        if (GUILayout.Button("Unlock Selected (Current Slot)"))
        {
            string id = loreIDs[selectedIndex];
            SaveManager.SetBossUnlock(id);
            Debug.Log($"Unlocked {id} in slot {SaveSlotManager.CurrentSlot}");
        }

        // Unlock all
        if (GUILayout.Button("Unlock ALL Lore Pages (Current Slot)"))
        {
            foreach (string id in loreIDs)
                SaveManager.SetBossUnlock(id);

            Debug.Log($"Unlocked ALL lore pages in slot {SaveSlotManager.CurrentSlot}");
        }

        GUILayout.Space(10);

        // Clear all
        if (GUILayout.Button("Clear ALL Lore Unlocks (All Slots)"))
        {
            ClearAllLore();
        }
    }

    private void RefreshLoreList()
    {
        bossLogMenu = FindObjectOfType<BossLogMenu>();
        loreIDs.Clear();

        if (bossLogMenu != null && bossLogMenu.loreEntries != null)
        {
            foreach (var entry in bossLogMenu.loreEntries)
            {
                if (!string.IsNullOrEmpty(entry.unlockID))
                    loreIDs.Add(entry.unlockID);
            }
        }

        Repaint();
    }

    private void ClearAllLore()
    {
        for (int slot = 1; slot <= 3; slot++)
        {
            foreach (var key in PlayerPrefsKeys.GetAllKeys())
            {
                if (key.StartsWith($"SaveSlot{slot}_BossUnlocked_"))
                    PlayerPrefs.DeleteKey(key);
            }
        }

        PlayerPrefs.Save();
        Debug.Log("Cleared ALL lore unlocks in ALL slots.");
    }
}
