using UnityEngine;

public class WormyBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float moveSpeed = 3f;
    public float turnChance = 0.02f;

    public GameObject hazardPrefab;

    private Vector2 direction = Vector2.right;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (!isAwake) return;

        if (Random.value < turnChance)
        {
            float angle = Random.Range(-90f, 90f);
            direction = Quaternion.Euler(0, 0, angle) * direction;
            direction.Normalize();
        }

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        if (Random.value < 0.01f)
            Instantiate(hazardPrefab, transform.position, Quaternion.identity);
    }

    public void WakeUp() => isAwake = true;
}
