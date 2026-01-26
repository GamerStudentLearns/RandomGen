using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Image img;

    [Header("Sprites")]
    public Sprite unvisitedSprite;
    public Sprite visitedSprite;
    public Sprite currentRoomSprite;

    private void Awake()
    {
        img = GetComponent<Image>();
        Hide(); // start hidden for fog-of-war
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

    public void SetUnvisited()
    {
        if (img == null) return;
        img.sprite = unvisitedSprite;
    }

    public void SetVisited()
    {
        if (img == null) return;
        img.sprite = visitedSprite;
    }

    public void SetAsCurrentRoom(bool isCurrent)
    {
        if (img == null) return;

        if (isCurrent)
            img.sprite = currentRoomSprite;
        else
            img.sprite = visitedSprite;
    }
}
