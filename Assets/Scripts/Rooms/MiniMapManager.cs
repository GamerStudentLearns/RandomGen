using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapManager : MonoBehaviour
{
    [Header("UI")]
    public RectTransform minimapPanel;
    public GameObject iconPrefab;

    [Header("Spacing")]
    public float iconSpacing = 20f;

    private Vector2Int gridCenter;
    private Dictionary<Vector2Int, MinimapIcon> icons = new Dictionary<Vector2Int, MinimapIcon>();

    public void Initialize(int gridSizeX, int gridSizeY)
    {
        gridCenter = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
    }

    public void RegisterRoom(Vector2Int roomIndex)
    {
        if (icons.ContainsKey(roomIndex))
            return;

        GameObject iconObj = Instantiate(iconPrefab, minimapPanel);
        iconObj.name = $"Icon-{roomIndex.x}-{roomIndex.y}";

        RectTransform rt = iconObj.GetComponent<RectTransform>();

        Vector2Int offsetIndex = roomIndex - gridCenter;

        rt.anchoredPosition = new Vector2(
            offsetIndex.x * iconSpacing,
            offsetIndex.y * iconSpacing
        );

        MinimapIcon icon = iconObj.GetComponent<MinimapIcon>();
        icons.Add(roomIndex, icon);
    }

    public void SetCurrentRoom(Vector2Int roomIndex)
    {
        foreach (var kvp in icons)
            kvp.Value.SetAsCurrentRoom(false);

        if (icons.ContainsKey(roomIndex))
            icons[roomIndex].SetAsCurrentRoom(true);
    }


    public MinimapIcon GetIcon(Vector2Int index)
    {
        if (icons.ContainsKey(index))
            return icons[index];

        return null;
    }

    public void ClearIcons()
    {
        foreach (var icon in icons.Values)
            Destroy(icon.gameObject);

        icons.Clear();
    }
}
