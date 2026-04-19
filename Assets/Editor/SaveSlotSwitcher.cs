using UnityEditor;
using UnityEngine;

public class SaveSlotSwitcher : EditorWindow
{
    private int selectedSlot = 1;

    [MenuItem("Tools/Save Slot Switcher")]
    public static void ShowWindow()
    {
        GetWindow<SaveSlotSwitcher>("Save Slot Switcher");
    }

    private void OnGUI()
    {
        GUILayout.Label("Switch Active Save Slot", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        selectedSlot = EditorGUILayout.IntSlider("Selected Slot", selectedSlot, 1, 3);

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply Slot"))
        {
            ApplySlot(selectedSlot);
        }

        EditorGUILayout.Space();

        GUILayout.Label($"Current Slot: {SaveSlotManager.CurrentSlot}", EditorStyles.helpBox);
    }

    private void ApplySlot(int slot)
    {
        SaveSlotManager.CurrentSlot = slot;
        Debug.Log($"[SaveSlotSwitcher] Active slot set to: {slot}");
    }
}
