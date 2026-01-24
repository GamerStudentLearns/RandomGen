using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public string sceneToLoad;
    public float loadDelay = 1f; // set delay in Inspector

    public void MenuLoader()
    {
        StartCoroutine(LoadSceneDelayed());
    }

    private System.Collections.IEnumerator LoadSceneDelayed()
    {
        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(sceneToLoad);
    }
}
