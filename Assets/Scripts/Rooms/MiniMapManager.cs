using UnityEngine;
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
    private HashSet<Vector2Int> visitedRooms = new HashSet<Vector2Int>();

    public void Initialize(int gridSizeX, int gridSizeY)
    {
        gridCenter = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
    }

    public void RegisterRoom(Vector2Int roomIndex)
    {
        if (icons.ContainsKey(roomIndex))
            return;

        GameObject iconObj = Instantiate(iconPrefab, minimapPanel);
        MinimapIcon icon = iconObj.GetComponent<MinimapIcon>();

        Room room = RoomManager.Instance.GetRoomScriptAt(roomIndex);
        if (room != null)
        {
            icon.iconType = room.roomType switch
            {
                Room.RoomType.Boss => MinimapIcon.IconType.Boss,
                Room.RoomType.Item => MinimapIcon.IconType.Item,
                _ => MinimapIcon.IconType.Normal
            };
        }

        icon.SetUnvisited();
        icons.Add(roomIndex, icon);

        // *** THIS PART WAS MISSING ***
        RectTransform rt = iconObj.GetComponent<RectTransform>();

        Vector2Int offsetIndex = roomIndex - gridCenter;

        rt.anchoredPosition = new Vector2(
            offsetIndex.x * iconSpacing,
            offsetIndex.y * iconSpacing
        );

        Debug.Log($"Room {roomIndex} type = {icon.iconType}");

    }



    public void SetCurrentRoom(Vector2Int roomIndex)
    {
        visitedRooms.Add(roomIndex);

        foreach (var kvp in icons)
        {
            Vector2Int index = kvp.Key;
            MinimapIcon icon = kvp.Value;

            if (index == roomIndex)
            {
                icon.SetAsCurrentRoom(true);
            }
            else
            {
                if (visitedRooms.Contains(index))
                    icon.SetVisited();
                else
                    icon.SetUnvisited(); // only changes sprite, not visibility
            }
        }
    }

    public MinimapIcon GetIcon(Vector2Int index)
    {
        if (icons.ContainsKey(index))
            return icons[index];

        return null;
    }
    public bool IsVisited(Vector2Int index)
    {
        return visitedRooms.Contains(index);
    }

    public void ClearIcons()
    {
        foreach (var icon in icons.Values)
            Object.Destroy(icon.gameObject);

        icons.Clear();
        visitedRooms.Clear();
    }
    public void MarkVisited(Vector2Int index)
    {
        visitedRooms.Add(index);

        if (icons.ContainsKey(index))
            icons[index].SetVisited();
    }

}
