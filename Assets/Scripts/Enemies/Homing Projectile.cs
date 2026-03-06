using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public Transform target;
    public float homingTime = 2f;

    private Projectile proj;
    private float timer;

    void Start()
    {
        proj = GetComponent<Projectile>();
        timer = homingTime;

        // Automatically find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    void Update()
    {
        if (target == null || proj == null) return;

        timer -= Time.deltaTime;

        if (timer > 0f)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            proj.SetDirection(dir);
        }
    }
}
