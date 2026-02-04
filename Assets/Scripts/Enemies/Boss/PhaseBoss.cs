using UnityEngine;

public class PhaseBoss : MonoBehaviour, IBoss
{
    public float chaseSpeed = 2f;
    public float attackCooldown = 2f;
    public float phaseDuration = 5f;

    public Transform player;
    public GameObject projectilePrefab;

    private float attackTimer;
    private float phaseTimer;
    private bool isAttackPhase = true;

    public bool isAwake = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        attackTimer = attackCooldown;
        phaseTimer = phaseDuration;
    }

    private void Update()
    {
        if (!isAwake) return;
        if (player == null) return;

        phaseTimer -= Time.deltaTime;

        if (isAttackPhase)
        {
            HandleAttacks();
        }
        else
        {
            ChasePlayer();
        }

        if (phaseTimer <= 0)
        {
            isAttackPhase = !isAttackPhase;
            phaseTimer = phaseDuration;
        }
    }

    private void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * chaseSpeed * Time.deltaTime;
    }

    private void HandleAttacks()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            ShootRadial();
            attackTimer = attackCooldown;
        }
    }

    private void ShootRadial()
    {
        int count = 12;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * step;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 5f;
        }
    }

    public void WakeUp()
    {
        isAwake = true;
        attackTimer = attackCooldown;
        phaseTimer = phaseDuration;
    }
}
