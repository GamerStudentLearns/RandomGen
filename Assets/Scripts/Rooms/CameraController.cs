using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool smoothSnap = false;
    public float smoothTime = 0.15f;
    private Vector3 velocity = Vector3.zero;

    public void SnapToRoom(Vector3 roomCenter)
    {
        Vector3 targetPos = new Vector3(roomCenter.x, roomCenter.y, transform.position.z);
        if (smoothSnap)
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        else
            transform.position = targetPos;
    }
}
