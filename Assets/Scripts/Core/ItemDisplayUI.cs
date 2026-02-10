using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayUI : MonoBehaviour
{
    public Transform iconContainer;
    public GameObject iconPrefab;

    private RunManager run;

    void Start()
    {
        run = RunManager.instance;

        RefreshIcons();

        RunManager.RunEvents.OnItemAcquired += RefreshIcons;
        RunManager.RunEvents.FloorChanged += RefreshIcons;
    }

    void OnDestroy()
    {
        RunManager.RunEvents.OnItemAcquired -= RefreshIcons;
        RunManager.RunEvents.FloorChanged -= RefreshIcons;
    }

    public void RefreshIcons()
    {
        if (run == null)
            return;

        foreach (Transform child in iconContainer)
            Destroy(child.gameObject);

        foreach (ItemData item in run.acquiredItems)
        {
            GameObject icon = Instantiate(iconPrefab, iconContainer);
            Image img = icon.GetComponent<Image>();
            img.sprite = item.icon;
        }
    }
}
