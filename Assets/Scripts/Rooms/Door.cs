using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public Vector2Int direction;
    private Room parentRoom;
    private bool canTeleport = false;

    void Awake()
    {
        parentRoom = GetComponentInParent<Room>();
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    public void EnableDoor()
    {
        canTeleport = true;
    }

    public void DisableDoorTemporarily(float duration)
    {
        canTeleport = false;
        StartCoroutine(ReenableAfterSeconds(duration));
    }

    IEnumerator ReenableAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
        canTeleport = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!canTeleport) return;

        canTeleport = false;
        StartCoroutine(TeleportPlayer(other.gameObject));
    }

    IEnumerator TeleportPlayer(GameObject player)
    {
        yield return null; // wait a frame for physics to settle
        parentRoom.EnterDoor(direction);
        yield return new WaitForSeconds(0.2f); // prevent immediate retrigger
        canTeleport = true;
    }
}
