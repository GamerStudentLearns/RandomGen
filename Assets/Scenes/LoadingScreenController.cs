using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject loadingCanvas;
    public float duration = 5f;

    private MonoBehaviour PlayerMovement;

    private void Start()
    {
        PlayerMovement = FindObjectOfType<PlayerMovement>();
        StartCoroutine(ShowLoadingScreen());
    }

    private System.Collections.IEnumerator ShowLoadingScreen()
    {
        loadingCanvas.SetActive(true);

        if (PlayerMovement != null)
            PlayerMovement.enabled = false;

        // FIX: unaffected by Time.timeScale
        yield return new WaitForSecondsRealtime(duration);

        loadingCanvas.SetActive(false);

        if (PlayerMovement != null)
            PlayerMovement.enabled = true;
    }
}
