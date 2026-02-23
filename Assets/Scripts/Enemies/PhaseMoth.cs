using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PhaseMoth : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public float hoverRadius = 2f;

    [Header("Phasing")]
    public float visibleDuration = 1.2f;
    public float invisibleDuration = 1.5f;
    public Sprite visibleSprite;
    public Sprite invisibleSprite;
    public SpriteRenderer spriteRenderer;

    private Transform player;
    private Collider2D col;
    private float phaseTimer;
    private bool isVisible;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        col = GetComponent<Collider2D>();
        isVisible = true;
        phaseTimer = visibleDuration;
    }

    void Update()
    {
        HandlePhasing();
        HoverAroundPlayer();
    }

    void HandlePhasing()
    {
        phaseTimer -= Time.deltaTime;
        if (phaseTimer <= 0f)
        {
            isVisible = !isVisible;
            phaseTimer = isVisible ? visibleDuration : invisibleDuration;

            if (col) col.enabled = isVisible;
            if (spriteRenderer)
                spriteRenderer.sprite = isVisible ? visibleSprite : invisibleSprite;
        }
    }

    void HoverAroundPlayer()
    {
        if (!player) return;

        Vector2 offset = (Vector2)(transform.position - player.position);
        if (offset.sqrMagnitude > hoverRadius * hoverRadius)
        {
            Vector2 dir = offset.normalized;
            transform.position -= (Vector3)(dir * moveSpeed * Time.deltaTime);
        }
    }
}
