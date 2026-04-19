using UnityEngine;
using TMPro;

public class BossLogMenu : MonoBehaviour
{
    [System.Serializable]
    public class LoreEntry
    {
        public string unlockID;          // e.g. "LorePage_Slot1"
        public GameObject buttonObject;  // The button in the scroll list
        public GameObject pageObject;    // The full lore page panel
    }

    [Header("Lore Entries")]
    public LoreEntry[] loreEntries;

    [Header("Lore Count UI")]
    public TMP_Text loreCountText;

    [Header("All Lore Page IDs (for counting + 100%)")]
    public string[] allLorePages;

    private void Start()
    {
        RefreshLoreButtons();
        UpdateLoreCount();
        HideAllPages();
    }

    // ---------------------------------------------------------
    // SHOW / HIDE BUTTONS BASED ON UNLOCKS
    // ---------------------------------------------------------
    public void RefreshLoreButtons()
    {
        foreach (var entry in loreEntries)
        {
            bool unlocked = SaveManager.AnySlotHasBossUnlock(entry.unlockID);

            if (entry.buttonObject != null)
                entry.buttonObject.SetActive(unlocked);
        }
    }

    // ---------------------------------------------------------
    // OPEN A LORE PAGE
    // ---------------------------------------------------------
    public void OpenPage(GameObject page)
    {
        HideAllPages();

        if (page != null)
            page.SetActive(true);
    }

    // ---------------------------------------------------------
    // CLOSE ALL LORE PAGES
    // ---------------------------------------------------------
    public void HideAllPages()
    {
        foreach (var entry in loreEntries)
        {
            if (entry.pageObject != null)
                entry.pageObject.SetActive(false);
        }
    }

    // ---------------------------------------------------------
    // UPDATE "Lore Pages Unlocked: X/Y"
    // ---------------------------------------------------------
    public void UpdateLoreCount()
    {
        int unlocked = 0;

        foreach (string id in allLorePages)
        {
            if (SaveManager.AnySlotHasBossUnlock(id))
                unlocked++;
        }

        if (loreCountText != null)
            loreCountText.text = $"Lore Pages Unlocked: {unlocked}/{allLorePages.Length}";
    }
}
