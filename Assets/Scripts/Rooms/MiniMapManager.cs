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

    // Auto-calculated center of the grid
    private Vector2Int gridCenter;

    // Stores icons by room index
    private Dictionary<Vector2Int, MinimapIcon> icons = new Dictionary<Vector2Int, MinimapIcon>();

    // Called by RoomManager after gridSizeX/Y are known
    public void Initialize(int gridSizeX, int gridSizeY)
    {
        gridCenter = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
    }

    public void RegisterRoom(Vector2Int roomIndex)
    {
        // Prevent duplicate icons
        if (icons.ContainsKey(roomIndex))
            return;

        GameObject iconObj = Instantiate(iconPrefab, minimapPanel);
        iconObj.name = $"Icon-{roomIndex.x}-{roomIndex.y}";

        RectTransform rt = iconObj.GetComponent<RectTransform>();

        // Offset so the minimap is centered
        Vector2Int offsetIndex = roomIndex - gridCenter;

        rt.anchoredPosition = new Vector2(
            offsetIndex.x * iconSpacing,
            offsetIndex.y * iconSpacing
        );

        MinimapIcon icon = iconObj.GetComponent<MinimapIcon>();
        icons.Add(roomIndex, icon);
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
