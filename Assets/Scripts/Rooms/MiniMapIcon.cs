using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Image img;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite currentRoomSprite;

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

    public void SetAsCurrentRoom(bool isCurrent)
    {
        if (img == null) return;

        img.sprite = isCurrent ? currentRoomSprite : normalSprite;
    }
}
