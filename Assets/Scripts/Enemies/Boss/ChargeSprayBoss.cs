using UnityEngine;

public class ChargeSprayBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float moveSpeed = 2f;
    public float chargeSpeed = 7f;
    public float chargeDuration = 0.5f;
    public float chargeCooldown = 2f;

    public GameObject projectilePrefab;

    private float chargeTimer;
    private bool charging = false;
    private Vector2 chargeDir;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        chargeTimer = chargeCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        if (charging)
        {
            transform.position += (Vector3)chargeDir * chargeSpeed * Time.deltaTime;
            chargeTimer -= Time.deltaTime;

            if (chargeTimer <= 0)
            {
                charging = false;
                chargeTimer = chargeCooldown;
                SprayCone();
            }
        }
        else
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;

            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                charging = true;
                chargeDir = (player.position - transform.position).normalized;
                chargeTimer = chargeDuration;
            }
        }
    }

    private void SprayCone()
    {
        for (int i = -2; i <= 2; i++)
        {
            float angle = i * 12f;
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 6f;
        }
    }

    public void WakeUp() => isAwake = true;
}
