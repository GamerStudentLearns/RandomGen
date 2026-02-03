using UnityEngine;
using UnityEngine.SceneManagement;

public class TimedSceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneName = "NextScene";

    [Header("Timer Settings")]
    [SerializeField] private float delay = 5f;

    private bool hasSwitched = false;

    private void Start()
    {
        StartCoroutine(SwitchSceneAfterDelay());
    }

    private void Update()
    {
        if (!hasSwitched && Input.GetKeyDown(KeyCode.Space))
        {
            SwitchSceneNow();
        }
    }

    private System.Collections.IEnumerator SwitchSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SwitchSceneNow();
    }

    private void SwitchSceneNow()
    {
        if (hasSwitched) return;
        hasSwitched = true;

        SceneManager.LoadScene(sceneName);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
