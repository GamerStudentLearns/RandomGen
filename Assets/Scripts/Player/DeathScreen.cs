using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public string menuScene;
    public string reloadName;

    public void ReloadLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(reloadName);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(menuScene);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
