using UnityEngine;

public class RapidBarrageBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject tearPrefab;

    public float fireRate = 0.12f;
    private float fireTimer;

    public float moveSpeed = 1.5f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        fireTimer = fireRate;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        Move();
        HandleFire();
    }

    private void Move()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void HandleFire()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            GameObject tear = Instantiate(tearPrefab, transform.position, Quaternion.identity);
            tear.GetComponent<Rigidbody2D>().linearVelocity = dir * 8f;

            fireTimer = fireRate;
        }
    }

    public void WakeUp() => isAwake = true;
}
