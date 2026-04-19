using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    public void SetFullscreen(bool value)
    {
        if (value)
        {
            // Unity 6 safe fullscreen call
            int w = Display.main.systemWidth;
            int h = Display.main.systemHeight;

            Screen.SetResolution(w, h, FullScreenMode.FullScreenWindow);
        }
        else
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
    }
}
