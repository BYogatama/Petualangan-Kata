using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class ArcadeManager : SceneLoader {

    #region Inspector

    [Header("Static Variable")]
    public int score;
    public int playerLife; 

    [Header("UI Elements")]
    public TMP_Text scoreValText;
    public TMP_Text timeValText;
    public Sprite[] hearthSprite;
    public Image healthStatus;

    [Header("Pause Menu")]
    public GameObject pauseWindow;
    public AudioMixer masterMixer;
    public AudioMixerSnapshot unpaused;
    public AudioMixerSnapshot paused;
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider effectVolume;
    public Button btnResume;
    public Button btnExit;

    [Header("Others")]
    public GameObject countDown;

    #endregion

    #region Privates Variabels

    int highScore;
    int time;

    int seconds;
    float timer;

    bool GameIsPaused = false;

    #endregion

    #region Unity Messages

    private void OnEnable()
    {
        btnResume.onClick.AddListener(delegate { Resume(); });
        btnExit.onClick.AddListener(delegate { Exit(); });

        masterVolume.onValueChanged.AddListener(delegate { SetVolume("masterVolume", masterVolume.value); });
        musicVolume.onValueChanged.AddListener(delegate { SetVolume("musicVolume", musicVolume.value); });
        effectVolume.onValueChanged.AddListener(delegate { SetVolume("effectVolume", effectVolume.value); });
    }

    IEnumerator Start()
    {
        while (!GameSettings.Instance.IsReady)
        {
            yield return null;
        }
        
        while (!GameSave.Instance.IsReady)
        {
            yield return null;
        }

        while (!AudioController.Instance.IsReady)
        {
            yield return null;
        }

        masterVolume.value = GameSettings.Instance.UserSettings.masterVolume;
        musicVolume.value = GameSettings.Instance.UserSettings.musicVolume;
        effectVolume.value = GameSettings.Instance.UserSettings.effectVolume;

        AudioController.Instance.MuteMusic("Elucidate", true);
        AudioController.Instance.PlayMusic("Jump!");
        StartCoroutine(Delay());
    }

    private void Update()
    {
        scoreValText.text = score.ToString();
        if(playerLife >= 0)
        {
            healthStatus.sprite = hearthSprite[playerLife];
            timer += Time.deltaTime;
            seconds = System.Convert.ToInt32(timer);
        }

        timeValText.text = seconds.ToString(); 

        PauseGame();
        PlayerLose();
    }

    #endregion

    #region Privates Methods

    private IEnumerator Delay()
    {
        Time.timeScale = 0;
        float pauseTime = Time.realtimeSinceStartup + 5f;
        while(Time.realtimeSinceStartup < pauseTime)
        {

            yield return 0;
        }
        countDown.gameObject.SetActive(false);
        Destroy(countDown);
        Time.timeScale = 1;
    }

    private void PlayerLose()
    {
        if(playerLife <= 0)
        {
            Time.timeScale = 0.5f;
            SaveGame();
            LoadNextScene("ArcadeSelesai");
        }
    }

    private void SaveGame()
    {

        var userSave = GameSave.Instance.LoadSaveGame(GameSave.Instance.PROFILE_NAME);

        if (userSave.arcadeHighScore > score)
        {
            highScore = userSave.arcadeHighScore;
        }
        else
        {
            highScore = score;
        }

        if(userSave.longestArcadetTime > seconds)
        {
            time = userSave.longestArcadetTime;
        }
        else
        {
            time = seconds;
        }

        GameSave.Instance.SaveArcadeGame(highScore, score, time, seconds);
    }

    private void SaveSettings()
    {
        int width = GameSettings.Instance.UserSettings.width;
        int height = GameSettings.Instance.UserSettings.height;
        bool fullScreen = GameSettings.Instance.UserSettings.fullScreen;

        GameSettings.Instance.SaveSettings(width, height, fullScreen, masterVolume.value, musicVolume.value, effectVolume.value);
    }

    private void SetVolume(string name, float volume)
    {
        masterMixer.SetFloat(name, volume);
    }

    private void LowPass()
    {
        if(Time.timeScale == 0)
        {
            paused.TransitionTo(.01f);
        }
        else
        {
            unpaused.TransitionTo(.01f);
        }
    }

    #endregion

    #region Menu Methods

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameIsPaused)
            {
                Pause();
            }
            else
            {
                SaveSettings();
                Resume();
            }
        }
    }

    private void Resume()
    {
        SaveSettings();
        pauseWindow.SetActive(false);
        Time.timeScale = 1;
        LowPass();
        GameIsPaused = false;
    }

    private void Pause()
    {
        pauseWindow.SetActive(true);
        Time.timeScale = 0;
        LowPass();
        GameIsPaused = true;
    }

    private void Exit()
    {
        Time.timeScale = 1;
        LowPass();
        SaveGame();
        LoadNextScene("ArcadeSelesai");
    }

    #endregion
}
