using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwapper : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad;
    public float delay = 2f;

    [Header("Cursor Settings")]
    [Tooltip("Enable this if the target scene is a menu.")]
    public bool unlockCursorOnLoad = true;

    private void Start()
    {
        Invoke(nameof(SwapScene), delay);
    }

    private void SwapScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneSwapper: No scene name assigned!");
            return;
        }

        if (unlockCursorOnLoad)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}
