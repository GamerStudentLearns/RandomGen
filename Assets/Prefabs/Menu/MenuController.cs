using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Levels To Load")]
    public string newGameLevel;
    private string levelToLoad;

    [Header("Audio Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text KeyboardSensitivityTextValue = null;
    [SerializeField] private Slider KeyboardSensitivitySlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainKeyboardSensitivity = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    private void Start()
    {
        LoadAudioSettings();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        if (volumeTextValue != null)
            volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ComfirmationBox(1f));
    }

    public void SetKeyboardSensitivity(float sensitivity)
    {
        mainKeyboardSensitivity = Mathf.RoundToInt(sensitivity);
        if (KeyboardSensitivityTextValue != null)
            KeyboardSensitivityTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle != null)
        {
            if (invertYToggle.isOn)
                PlayerPrefs.SetInt("masterInvertY", 1);
            else
                PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainKeyboardSensitivity);
        StartCoroutine(ComfirmationBox(1f));
    }

    public void NewGameDialogYes()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(newGameLevel);
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
    }

    public IEnumerator ComfirmationBox(float duration)
    {
        if (comfirmationPrompt != null)
        {
            comfirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(duration);
            comfirmationPrompt.SetActive(false);
        }
    }
}
