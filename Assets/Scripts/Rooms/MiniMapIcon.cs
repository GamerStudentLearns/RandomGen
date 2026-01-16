using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
        Hide();
    }

    public void Hide()
    {
        if (img != null)
            img.enabled = false;
    }

    public void Reveal()
    {
        if (img != null)
            img.enabled = true;
    }
}
