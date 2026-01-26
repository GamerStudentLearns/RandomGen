using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        // Listen for scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset ONLY when Scene 1 loads
        if (scene.buildIndex == 1)
        {
            ResetRun();
        }
    }

    public void ResetRun()
    {
        acquiredItems.Clear();
        currentHearts = maxHearts;
        Debug.Log("Run reset — item pool restored.");
    }
}
