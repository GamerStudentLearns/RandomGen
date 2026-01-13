using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class HeartUI : MonoBehaviour
{
    [Header("Heart Setup")]
    public GameObject heartPrefab;     // UI Image prefab
    public Sprite fullHeart;
    public Sprite emptyHeart;
    private List<Image> hearts = new List<Image>();
    public void Initialize(int maxHearts)
    {
        // Clear old hearts
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        hearts.Clear();
        // Create heart images
        for (int i = 0; i < maxHearts; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, transform);
            Image img = heartObj.GetComponent<Image>();
            hearts.Add(img);
        }
    }
    public void UpdateHearts(int currentHearts)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < currentHearts ? fullHeart : emptyHeart;
        }
    }
}