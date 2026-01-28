using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartUI : MonoBehaviour
{
    [Header("Heart Setup")]
    public GameObject heartPrefab;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite soulHeart;

    private List<Image> hearts = new List<Image>();

    private int cachedMaxHearts;
    private int cachedSoulHearts;

    public void Initialize(int maxHearts, int soulHearts = 0)
    {
        cachedMaxHearts = maxHearts;
        cachedSoulHearts = soulHearts;

        int total = maxHearts + soulHearts;

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < total; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, transform);
            Image img = heartObj.GetComponent<Image>();
            hearts.Add(img);
        }
    }

    public void UpdateHearts(int currentHearts)
    {
        UpdateHearts(currentHearts, cachedSoulHearts);
    }

    public void UpdateHearts(int currentHearts, int soulHearts)
    {
        cachedSoulHearts = soulHearts;

        int total = cachedMaxHearts + soulHearts;

        // Ensure UI has correct number of slots
        if (hearts.Count != total)
            Initialize(cachedMaxHearts, soulHearts);

        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHearts)
            {
                hearts[i].sprite = fullHeart; // red heart
            }
            else if (i < cachedMaxHearts)
            {
                hearts[i].sprite = emptyHeart; // empty red container
            }
            else
            {
                hearts[i].sprite = soulHeart; // soul heart (extra)
            }
        }
    }
}
