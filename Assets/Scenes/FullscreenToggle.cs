using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }
}
