using UnityEngine;

public class RoomTriggerForwarder : MonoBehaviour
{
    public Room parentRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            parentRoom.PlayerEnteredRoom();
    }
}
