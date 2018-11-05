using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsLoader : MonoBehaviour {

    [Header("Scene to Load")]
    public GameObject title;
    public int nextScene = 1;

    [Header("Audio Mixer")]
    public AudioMixer masterMixer;

    [Header("Loading Screen Component")]
    public GameObject loadingScreen;
    public Slider progressBar;
    public TMP_Text progressText;

    private FadeLevel FadeLevel;

    IEnumerator Start()
    {
        while (!GameSettings.Instance.IsReady)
        {
            yield return null;
        }

        while (!AudioController.Instance.IsReady)
        {
            yield return null;
        }
        
        masterMixer.SetFloat("masterVolume", GameSettings.Instance.UserSettings.masterVolume);
        masterMixer.SetFloat("musicVolume", GameSettings.Instance.UserSettings.musicVolume);
        masterMixer.SetFloat("effectVolume", GameSettings.Instance.UserSettings.effectVolume);

        FadeLevel = FindObjectOfType<FadeLevel>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            title.SetActive(false);
            loadingScreen.SetActive(true);
            FadeLevel.FadeOut();
            LoadLevel(nextScene);
        }
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsychronously(sceneIndex));
    }

    IEnumerator LoadAsychronously(int sceneIndex)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneIndex);

        while (!loading.isDone)
        {
            float progress = Mathf.Clamp01(loading.progress / .9f);
            progressBar.value = progress;
            progressText.text = string.Format("{0:0.##}", progress * 100f) + " %";
            yield return null;
        }
    }
}
