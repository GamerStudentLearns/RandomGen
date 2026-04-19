using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("CraigStoryButton")]
    public GameObject specialButton;

    [Header("Save Slot Status Text")]
    public TMP_Text slot1Status;
    public TMP_Text slot2Status;
    public TMP_Text slot3Status;

    [Header("Levels To Load")]
    public string newGameLevel;

    [Header("Audio Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;
    [SerializeField] private GameObject comfirmationPrompt = null;

    [SerializeField] private Toggle damageSoundToggle = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text KeyboardSensitivityTextValue = null;
    [SerializeField] private Slider KeyboardSensitivitySlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainKeyboardSensitivity = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Save Slot UI")]
    public TMP_Text selectedSlotText;

    [Header("Global Unlock Reward (TRUE 100%)")]
    public GameObject globalUnlockObject;

    // ⭐ ALL lore pages that count toward 100%
    public string[] allLorePages = {
        "LorePage_Slot1",
        "LorePage_Slot2",
        "LorePage_Slot3"
    };

    private void Start()
    {
        specialButton.SetActive(SaveManager.AnySlotHasSpecialUnlocked());
        RefreshGlobalUnlocks();
        LoadAudioSettings();
        LoadLastUsedSlot();
        UpdateSlotStatusLabels();
    }

    // -----------------------------
    // SAVE SLOT SYSTEM
    // -----------------------------
    public void SelectSaveSlot(int slot)
    {
        SaveSlotManager.CurrentSlot = slot;
        PlayerPrefs.SetInt("LastUsedSlot", slot);
        PlayerPrefs.Save();

        if (selectedSlotText != null)
            selectedSlotText.text = "Selected Slot: " + slot;

        UpdateSlotStatusLabels();
        RefreshGlobalUnlocks();
    }

    private void LoadLastUsedSlot()
    {
        int lastSlot = PlayerPrefs.GetInt("LastUsedSlot", 1);
        SaveSlotManager.CurrentSlot = lastSlot;

        if (selectedSlotText != null)
            selectedSlotText.text = "Selected Slot: " + lastSlot;
    }

    public void NewGameDialogYes()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(newGameLevel);
    }

    // -----------------------------
    // AUDIO SETTINGS
    // -----------------------------
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        if (volumeTextValue != null)
            volumeTextValue.text = volume.ToString("0.0");
    }

    public void SetDamageSound(bool enabled)
    {
        PlayerPrefs.SetInt("DamageSoundEnabled", enabled ? 1 : 0);
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);

        if (damageSoundToggle != null)
            PlayerPrefs.SetInt("DamageSoundEnabled", damageSoundToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
        StartCoroutine(ComfirmationBox(1f));
    }

    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float volume = PlayerPrefs.GetFloat("masterVolume");
            AudioListener.volume = volume;

            if (volumeSlider != null)
                volumeSlider.value = volume;
            if (volumeTextValue != null)
                volumeTextValue.text = volume.ToString("0.0");
        }

        if (damageSoundToggle != null)
            damageSoundToggle.isOn = PlayerPrefs.GetInt("DamageSoundEnabled", 1) == 1;
    }

    // -----------------------------
    // GAMEPLAY SETTINGS
    // -----------------------------
    public void SetKeyboardSensitivity(float sensitivity)
    {
        mainKeyboardSensitivity = Mathf.RoundToInt(sensitivity);

        if (KeyboardSensitivityTextValue != null)
            KeyboardSensitivityTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle != null)
            PlayerPrefs.SetInt("masterInvertY", invertYToggle.isOn ? 1 : 0);

        PlayerPrefs.SetFloat("masterSen", mainKeyboardSensitivity);
        StartCoroutine(ComfirmationBox(1f));
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;

            if (volumeSlider != null)
                volumeSlider.value = defaultVolume;
            if (volumeTextValue != null)
                volumeTextValue.text = defaultVolume.ToString("0.0");

            if (damageSoundToggle != null)
                damageSoundToggle.isOn = true;

            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            if (KeyboardSensitivityTextValue != null)
                KeyboardSensitivityTextValue.text = defaultSen.ToString("0");
            if (KeyboardSensitivitySlider != null)
                KeyboardSensitivitySlider.value = defaultSen;

            mainKeyboardSensitivity = defaultSen;

            if (invertYToggle != null)
                invertYToggle.isOn = false;

            GameplayApply();
        }
    }

    // -----------------------------
    // CONFIRMATION POPUP
    // -----------------------------
    public IEnumerator ComfirmationBox(float duration)
    {
        if (comfirmationPrompt != null)
        {
            comfirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(duration);
            comfirmationPrompt.SetActive(false);
        }
    }

    // -----------------------------
    // QUIT GAME
    // -----------------------------
    public void QuitGame()
    {
        Application.Quit();
    }

    // -----------------------------
    // SLOT LABELS
    // -----------------------------
    public void UpdateSlotStatusLabels()
    {
        int previousSlot = SaveSlotManager.CurrentSlot;

        // SLOT 1
        SaveSlotManager.CurrentSlot = 1;
        slot1Status.text =
            SaveManager.HasUnlockedSpecialButton() ? "Truth Seeker" :
            SaveManager.HasClearedLevel6() ? "Dead Man Walking" :
            "New Game";

        // SLOT 2
        SaveSlotManager.CurrentSlot = 2;
        slot2Status.text =
            SaveManager.HasUnlockedSpecialButton() ? "Truth Seeker" :
            SaveManager.HasClearedLevel6() ? "Dead Man Walking" :
            "New Game";

        // SLOT 3
        SaveSlotManager.CurrentSlot = 3;
        slot3Status.text =
            SaveManager.HasUnlockedSpecialButton() ? "Truth Seeker" :
            SaveManager.HasClearedLevel6() ? "Dead Man Walking" :
            "New Game";

        SaveSlotManager.CurrentSlot = previousSlot;
    }

    // -----------------------------
    // DELETE SLOT
    // -----------------------------
    public void DeleteSaveSlot(int slot)
    {
        SaveManager.DeleteSlot(slot);
        UpdateSlotStatusLabels();
        RefreshGlobalUnlocks();
    }

    // -----------------------------
    // SPECIAL BUTTON
    // -----------------------------
    public void RefreshSpecialButton()
    {
        specialButton.SetActive(SaveManager.AnySlotHasSpecialUnlocked());
    }

    // -----------------------------
    // ⭐ TRUE 100% COMPLETION
    // -----------------------------
    public void RefreshGlobalUnlocks()
    {
        if (globalUnlockObject != null)
        {
            bool has100 = SaveManager.Has100PercentCompletion(allLorePages);
            globalUnlockObject.SetActive(has100);
        }
    }
}
