using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Toggle FullScreenToggle;
    Resolution[] AllResolutions;
    bool IsFullScreen;
    int SelectedResolution;
    List<Resolution> SelectedResolutionList = new List<Resolution>();





    void Start()
    {
        IsFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            {
                newRes = res.width + " x " + res.height;
                if (!resolutionStringList.Contains(newRes))
                {
                    resolutionStringList.Add(newRes);
                    SelectedResolutionList.Add(res);
                }
            }

            ResDropDown.AddOptions(resolutionStringList);
        }
    }

    public void ChangeResolution()
    {
        SelectedResolution = ResDropDown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }

    public void ChangeFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }

    
    void Update()
    {
        
    }
}
