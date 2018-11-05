using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [Header("Loading Screen Component")]
    public GameObject loadingCanvas;
    public Slider progressBar;
    public TMP_Text progressText;
    public FadeLevel fadeLevel;

    #region Shared Methods

    protected void LoadNextScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }
    
    #endregion
    
    #region Private Methods

    IEnumerator LoadScene(string sceneName)
    {
        loadingCanvas.SetActive(true);
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);

        while (!loading.isDone)
        {
            float progress = Mathf.Clamp01(loading.progress / .9f);
            progressBar.value = progress;
            progressText.text = string.Format("{0:0.##}", progress * 100f) + " %";
            yield return null;
        }
        
        fadeLevel.FadeOut();
    }

    #endregion

}
