using UnityEngine;

public class MargarineBoss : MonoBehaviour
{
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float chargeSpeed = 7f;

    [Header("Charge Attack")]
    public float chargeDuration = 0.5f;
    public float attackCooldown = 2.5f;

    private float attackTimer;
    private bool charging = false;
    private Vector2 chargeDir;

    [HideInInspector] public bool isAwake = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        attackTimer = attackCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null)
            return;

        attackTimer -= Time.deltaTime;

        if (charging)
        {
            transform.position += (Vector3)(chargeDir * chargeSpeed * Time.deltaTime);
            return;
        }

        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

        if (attackTimer <= 0)
        {
            StartCharge();
            attackTimer = attackCooldown;
        }
    }

    private void StartCharge()
    {
        charging = true;
        chargeDir = (player.position - transform.position).normalized;
        Invoke(nameof(StopCharge), chargeDuration);
    }

    private void StopCharge()
    {
        charging = false;
    }
}
