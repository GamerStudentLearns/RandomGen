using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapper : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene you want to load.")]
    public string sceneToLoad;

    [Tooltip("How long to wait before swapping scenes.")]
    public float delay = 2f;

    private void Start()
    {
        // Start the delayed scene load
        Invoke(nameof(SwapScene), delay);
    }

    private void SwapScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneSwapper: No scene name assigned!");
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
