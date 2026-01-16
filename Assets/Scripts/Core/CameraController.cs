using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float roomWidth = 20f;
    public float roomHeight = 12f;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;

        // Fit room vertically
        cam.orthographicSize = roomHeight / 2f;

        // Force aspect ratio to match the room
        float targetAspect = roomWidth / roomHeight;
        float windowAspect = (float)Screen.width / Screen.height;

        float scale = windowAspect / targetAspect;

        if (scale < 1f)
        {
            // Screen is too tall → add letterbox bars top/bottom
            cam.rect = new Rect(0f, (1f - scale) / 2f, 1f, scale);
        }
        else
        {
            // Screen is too wide → add pillarbox bars left/right
            float invScale = 1f / scale;
            cam.rect = new Rect((1f - invScale) / 2f, 0f, invScale, 1f);
        }
    }
}
