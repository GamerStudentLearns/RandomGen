using UnityEngine;

public class EyeLordBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject tearPrefab;
    public GameObject eyePrefab;

    public float moveSpeed = 1.2f;
    public float eyeOrbitRadius = 2f;
    public float eyeOrbitSpeed = 90f;

    private GameObject leftEye;
    private GameObject rightEye;

    public float fireCooldown = 1.5f;
    private float fireTimer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Parent the eyes to THIS boss so they get destroyed with it
        leftEye = Instantiate(eyePrefab, transform.position, Quaternion.identity, transform);
        rightEye = Instantiate(eyePrefab, transform.position, Quaternion.identity, transform);

        fireTimer = fireCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        Move();
        OrbitEyes();
        HandleFire();
    }

    private void Move()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void OrbitEyes()
    {
        float t = Time.time * eyeOrbitSpeed * Mathf.Deg2Rad;

        leftEye.transform.position =
            transform.position + new Vector3(Mathf.Cos(t), Mathf.Sin(t), 0) * eyeOrbitRadius;

        rightEye.transform.position =
            transform.position + new Vector3(Mathf.Cos(t + Mathf.PI), Mathf.Sin(t + Mathf.PI), 0) * eyeOrbitRadius;
    }

    private void HandleFire()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0) return;

        FireDiagonal(leftEye.transform.position);
        FireDiagonal(rightEye.transform.position);

        fireTimer = fireCooldown;
    }

    private void FireDiagonal(Vector3 pos)
    {
        Vector2[] dirs = {
            new Vector2(1,1).normalized,
            new Vector2(-1,1).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(-1,-1).normalized
        };

        foreach (var d in dirs)
        {
            GameObject tear = Instantiate(tearPrefab, pos, Quaternion.identity);
            tear.GetComponent<Rigidbody2D>().linearVelocity = d * 5f;
        }
    }

    public void WakeUp() => isAwake = true;

    // -------------------------
    // DEATH HANDLER
    // -------------------------
    public void Die()
    {
        // Eyes are children now, but we destroy them explicitly anyway
        if (leftEye != null)
            Destroy(leftEye);

        if (rightEye != null)
            Destroy(rightEye);

        Destroy(gameObject);
    }
}
