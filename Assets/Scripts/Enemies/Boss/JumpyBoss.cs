using UnityEngine;

public class JumpyBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float jumpCooldown = 2f;
    private float jumpTimer;

    public float jumpSpeed = 6f;
    public float jumpDuration = 0.5f;

    public GameObject puddlePrefab;

    private bool jumping = false;
    private Vector3 targetPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        jumpTimer = jumpCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        if (jumping)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, jumpSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                jumping = false;
                Instantiate(puddlePrefab, transform.position, Quaternion.identity);
            }
        }
        else
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0)
            {
                StartJump();
                jumpTimer = jumpCooldown;
            }
        }
    }

    private void StartJump()
    {
        targetPos = player.position;
        jumping = true;
    }

    public void WakeUp() => isAwake = true;
}
