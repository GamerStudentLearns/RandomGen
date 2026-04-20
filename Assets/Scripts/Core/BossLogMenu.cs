using UnityEngine;
using TMPro;
using System.Collections;   // <-- Needed for IEnumerator

public class BossLogMenu : MonoBehaviour
{
    [System.Serializable]
    public class LoreEntry
    {
        public string unlockID;
        public GameObject buttonObject;
        public GameObject pageObject;
    }

    [Header("Lore Entries")]
    public LoreEntry[] loreEntries;

    [Header("Lore Count UI")]
    public TMP_Text loreCountText;

    [Header("All Lore Page IDs")]
    public string[] allLorePages;

    [Header("Exit Delay")]
    public float exitDelay = 2f;

    private bool inputLocked = false;

    private void Start()
    {
        RefreshLoreButtons();
        UpdateLoreCount();
        HideAllPages();
    }

    public void RefreshLoreButtons()
    {
        foreach (var entry in loreEntries)
        {
            bool unlocked = SaveManager.AnySlotHasBossUnlock(entry.unlockID);

            if (entry.buttonObject != null)
                entry.buttonObject.SetActive(unlocked);
        }
    }

    public void OpenPage(GameObject page)
    {
        if (inputLocked) return;

        HideAllPages();

        if (page != null)
            page.SetActive(true);
    }

    public void ExitMenu()
    {
        StartCoroutine(ExitDelayRoutine());
    }

    private IEnumerator ExitDelayRoutine()
    {
        inputLocked = true;

        yield return new WaitForSeconds(exitDelay);

        inputLocked = false;

        gameObject.SetActive(false);
    }

    public void HideAllPages()
    {
        foreach (var entry in loreEntries)
        {
            if (entry.pageObject != null)
                entry.pageObject.SetActive(false);
        }
    }

    public void UpdateLoreCount()
    {
        int unlocked = 0;

        foreach (string id in allLorePages)
        {
            bool has = SaveManager.AnySlotHasBossUnlock(id);

            if (has)
                unlocked++;
        }

        if (loreCountText != null)
            loreCountText.text = $"Lore Pages Unlocked: {unlocked}/{allLorePages.Length}";
    }
}
