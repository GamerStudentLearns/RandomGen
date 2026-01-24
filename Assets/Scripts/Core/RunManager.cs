using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

    public int currentHearts;
    public int maxHearts;

    public List<ItemData> acquiredItems = new List<ItemData>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetRun()
    {
        acquiredItems.Clear();
        currentHearts = maxHearts;
        Debug.Log("Run reset — item pool restored.");
    }
}
