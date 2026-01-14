using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Doors")]
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    [Header("Enemies")]
    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] int minEnemies = 1;
    [SerializeField] int maxEnemies = 4;

    [HideInInspector] public Vector2Int RoomIndex;
    [HideInInspector] public bool IsStartingRoom = false;
    [HideInInspector] public RoomManager RoomManager;

    private bool playerHasEntered = false;

    void Start()
    {
        // Ensure doors start disabled
        DisableDoorsTemporarily(0.1f);
    }

    public void OnPlayerEnter()
    {
        if (playerHasEntered) return;
        playerHasEntered = true;

        if (!IsStartingRoom)
            SpawnEnemies();

        EnableAllDoors();
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0) return;

        int count = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + new Vector3(
                Random.Range(-7f, 7f),
                Random.Range(-4f, 4f),
                0
            );
            Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], pos, Quaternion.identity, transform);
        }
    }

    public void OpenDoor(Vector2Int direction)
    {
        if (direction == Vector2Int.up) topDoor.SetActive(true);
        if (direction == Vector2Int.down) bottomDoor.SetActive(true);
        if (direction == Vector2Int.left) leftDoor.SetActive(true);
        if (direction == Vector2Int.right) rightDoor.SetActive(true);
    }

    public Vector3 GetDoorPosition(Vector2Int dir)
    {
        if (dir == Vector2Int.up && bottomDoor != null) return bottomDoor.transform.position + new Vector3(0, 0.5f, 0);
        if (dir == Vector2Int.down && topDoor != null) return topDoor.transform.position + new Vector3(0, -0.5f, 0);
        if (dir == Vector2Int.left && rightDoor != null) return rightDoor.transform.position + new Vector3(-0.5f, 0, 0);
        if (dir == Vector2Int.right && leftDoor != null) return leftDoor.transform.position + new Vector3(0.5f, 0, 0);
        return transform.position; // fallback
    }

    public void EnableAllDoors()
    {
        Door[] doors = GetComponentsInChildren<Door>();
        foreach (Door d in doors) d.EnableDoor();
    }

    public void DisableDoorsTemporarily(float duration)
    {
        Door[] doors = GetComponentsInChildren<Door>();
        foreach (Door d in doors) d.DisableDoorTemporarily(duration);
    }

    public void EnterDoor(Vector2Int direction)
    {
        RoomManager.MovePlayerToRoom(this, direction);
    }
}
