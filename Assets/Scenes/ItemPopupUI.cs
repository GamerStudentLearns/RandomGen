using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemPopupUI : MonoBehaviour
{
    public static ItemPopupUI instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        instance = this;
        panel.SetActive(false);
    }

    public void Show(ItemData item)
    {
        itemNameText.text = item.itemName;
        descriptionText.text = item.description;

        panel.SetActive(true);

        // Optional: auto-hide after 3 seconds
        CancelInvoke(nameof(Hide));
        Invoke(nameof(Hide), 3f);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
