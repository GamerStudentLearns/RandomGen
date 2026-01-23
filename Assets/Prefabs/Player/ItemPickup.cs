using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item;

    [Header("Pedestal")]
    public Sprite emptyPedestalSprite;

    private bool pickedUp = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        if (other.TryGetComponent(out PlayerStats stats))
        {
            item.Apply(stats);
            BecomeEmptyPedestal();
        }
    }

    private void BecomeEmptyPedestal()
    {
        pickedUp = true;

        spriteRenderer.sprite = emptyPedestalSprite;
        col.enabled = false;
    }
}
