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

    private void Start()
    {
        // If this item was already acquired earlier in the run,
        // instantly convert it to an empty pedestal.
        if (RunManager.instance.acquiredItems.Contains(item))
        {
            BecomeEmptyPedestal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;

        if (other.TryGetComponent(out PlayerStats stats))
        {
            // Prevent duplicate pickups
            if (RunManager.instance.acquiredItems.Contains(item))
            {
                BecomeEmptyPedestal();
                return;
            }

            // ONE-TIME pickup effect
            item.OnPickup(stats, RunManager.instance);

            // Save item for future floors
            RunManager.instance.acquiredItems.Add(item);

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
