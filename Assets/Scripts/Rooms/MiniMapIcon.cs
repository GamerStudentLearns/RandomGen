using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Image img;

    public enum IconType { Normal, Boss, Item }
    public IconType iconType = IconType.Normal;

    [Header("Normal Room Sprites")]
    public Sprite normalUnvisited;
    public Sprite normalVisited;
    public Sprite normalCurrent;

    [Header("Boss Room Sprites")]
    public Sprite bossUnvisited;
    public Sprite bossVisited;
    public Sprite bossCurrent;

    [Header("Item Room Sprites")]
    public Sprite itemUnvisited;
    public Sprite itemVisited;
    public Sprite itemCurrent;

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

    public void SetUnvisited()
    {
        if (img == null) return;

        img.sprite = iconType switch
        {
            IconType.Boss => bossUnvisited,
            IconType.Item => itemUnvisited,
            _ => normalUnvisited
        };
    }

    public void SetVisited()
    {
        if (img == null) return;

        img.sprite = iconType switch
        {
            IconType.Boss => bossVisited,
            IconType.Item => itemVisited,
            _ => normalVisited
        };
    }

    public void SetAsCurrentRoom(bool isCurrent)
    {
        if (img == null) return;

        if (!isCurrent)
        {
            SetVisited();
            return;
        }

        img.sprite = iconType switch
        {
            IconType.Boss => bossCurrent,
            IconType.Item => itemCurrent,
            _ => normalCurrent
        };
    }
}
