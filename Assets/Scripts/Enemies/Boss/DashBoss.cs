using UnityEngine;

public class DashBoss : MonoBehaviour, IBoss
{
    public float moveSpeed = 1.5f;
    public float dashSpeed = 8f;
    public float dashDuration = 0.4f;
    public float restDuration = 1.2f;

    public Transform player;
    public bool isAwake = false;

    private float stateTimer;
    private bool isDashing;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stateTimer = restDuration;
    }

    private void Update()
    {
        if (!isAwake) return;
        if (player == null) return;

        stateTimer -= Time.deltaTime;

        if (isDashing)
        {
            DashMovement();
            if (stateTimer <= 0)
            {
                isDashing = false;
                stateTimer = restDuration;
            }
        }
        else
        {
            MoveTowardPlayer();
            if (stateTimer <= 0)
            {
                isDashing = true;
                stateTimer = dashDuration;
            }
        }
    }

    private void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void DashMovement()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * dashSpeed * Time.deltaTime;
    }

    public void WakeUp()
    {
        isAwake = true;
    }
}
