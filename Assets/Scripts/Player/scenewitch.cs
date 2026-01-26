using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedSceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName = "NextScene";

    [Header("Timer Settings")]
    [SerializeField] private float delay = 5f;

    private void Start()
    {
        StartCoroutine(SwitchScene());
    }

    private System.Collections.IEnumerator SwitchScene()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
