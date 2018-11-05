using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class EndlessManager : SceneLoader  {

    #region Inspector Window

    [Header("Level")]
    public string levelName;
    
    [Header("Time & Score")]
    public int score;
    public int time;
    public TMP_Text scoreText;
    public TMP_Text timeText;

    [Header("Player Stats")]
    public GameObject player;
    public int playerLife;
    public int maxPlayerHealth;
    public int curPlayerHealth;
    public int playerMinDamage;
    public int playerMaxDamage;
    public Vector3 playerRespawnPos;

    [Header("Enemy Stats")]
    public int maxEnemyHealth;
    public int curEnemyHealth;
    public int enemyMinDamage;
    public int enemyMaxDamage;

    [Header("Player User Interface")]
    public Sprite[] playerStatusSprite;
    public Sprite[] playerLifeSprite;
    public Image playerStatus;
    public Image playerLifeValue;
    public TMP_Text playerHealthText;
    public Slider playerHealthBar;

    [Header("Enemy User Interface")]
    public Slider enemyHealthBar;
    public TMP_Text enemyHealthText;

    [Header("Scene")]
    public GameObject mainScene;
    public GameObject battleScene;

    [Header("User Interface")]
    public GameObject mainUI;
    public GameObject battleUI;
    public GameObject gameOverUI;
    public TMP_Text gameOverStatus;

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
    
    [Header("Battle Reference")]
    public bool isInBattle;
    public GameObject refPlayer;
    public GameObject refEnemy;

    #endregion

    #region Privates Variabels

    bool GameIsPaused;

    int highScore;
    int totalScore;
    int timeLeft;

    float timer;

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

    private void Awake()
    {
        mainUI.SetActive(true);
        battleUI.SetActive(false);
    
        playerHealthBar.value = curPlayerHealth;
        playerHealthText.text = curPlayerHealth.ToString();

        curPlayerHealth = maxPlayerHealth;
        timer = time;
    }

    private IEnumerator Start()
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

        GameSave.Instance.LEVEL_NAME = levelName;

        masterVolume.value = GameSettings.Instance.UserSettings.masterVolume;
        musicVolume.value = GameSettings.Instance.UserSettings.musicVolume;
        effectVolume.value = GameSettings.Instance.UserSettings.effectVolume;

        AudioController.Instance.PlayMusic("Jump!");
        AudioController.Instance.PlayMusic("Crack The Code");

        AudioController.Instance.MuteMusic("Crack The Code", true);
        AudioController.Instance.MuteMusic("Elucidate", true);

        LevelGeneration levelGen = FindObjectOfType<LevelGeneration>();
        if (levelGen.LevelCreationFinished)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerRespawnPos = player.transform.position;
        }
        else
        {
            yield return null;
        }
    }

    private void Update()
    {
        HandleInput();
        PlayerLife();
        
        scoreText.text = score.ToString();

        if (!isInBattle)
        {
            playerHealthBar.value = curPlayerHealth;
            playerHealthText.text = curPlayerHealth.ToString();
                        
            timer -= Time.deltaTime;
            time = Convert.ToInt32(timer);
            timeText.text = time.ToString() + " s";
            if (time <= 0)
            {
                time = 0;
                gameOverStatus.text = "WAKTU KAMU HABIS";
                StartCoroutine(GameOver());
            }
        }

        PlayerStatus();
    }

    #endregion

    #region Menu Methods

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

        AudioController.Instance.StopMusic("Jump!");
        AudioController.Instance.StopMusic("Crack The Code");
        AudioController.Instance.MuteMusic("Elucidate", false);

        LoadNextScene("PilihLevel");
    }

    #endregion
    
    #region Publics Methods

    public void Respawn(Vector3 respawnPoint)
    {
        player.transform.position = new Vector3(respawnPoint.x, respawnPoint.y, respawnPoint.z);
        playerLife -= 1;
        curPlayerHealth = maxPlayerHealth;
    }

    public void LoadScene(string scene)
    {
        timeLeft = time;
        totalScore = (timeLeft * 10) + score;
        SaveGame();

        AudioController.Instance.StopMusic("Jump!");
        AudioController.Instance.StopMusic("Crack The Code");
        AudioController.Instance.MuteMusic("Elucidate", false);

        LoadNextScene(scene);
    }

    #endregion

    #region Privates Methods
    
    private void PlayerStatus()
    {
        if (curPlayerHealth <= 20)
        {
            playerStatus.sprite = playerStatusSprite[2];
        }
        else if (curPlayerHealth <= 50)
        {
            playerStatus.sprite = playerStatusSprite[1];
        }
        else if (curPlayerHealth <= 100)
        {
            playerStatus.sprite = playerStatusSprite[0];
        }
    }

    private void PlayerLife()
    {
        if (playerLife >= 0)
        {
            playerLifeValue.sprite = playerLifeSprite[playerLife];
        }

        if (curPlayerHealth > maxPlayerHealth)
        {
            curPlayerHealth = maxPlayerHealth;
        }

        if (curPlayerHealth <= 0)
        {
            if (playerLife != 0 || playerLife < 0)
            {
                Respawn(playerRespawnPos);
            }
            else
            {
               StartCoroutine(GameOver());
            }
        }
    }

    private void HandleInput()
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
    
    private void SaveGame()
    {
        var userSave = GameSave.Instance.LoadSaveGame(GameSave.Instance.PROFILE_NAME);

        if (levelName == "Padang Rumput")
        {
            if(userSave.pr_highScore > totalScore)
            {
                highScore = userSave.pr_highScore;
            }
            else
            {
                highScore = totalScore;
            }
        }
        else if (levelName == "Pegunungan")
        {
            if (userSave.pe_highScore > totalScore)
            {
                highScore = userSave.pe_highScore;
            }
            else
            {
                highScore = totalScore;
            }
        }
        else if (levelName == "Gurun")
        {
            if (userSave.gr_highScore > totalScore)
            {
                highScore = userSave.gr_highScore;
            }
            else
            {
                highScore = totalScore;
            }
        }

        GameSave.Instance.SaveEndlessGame(levelName, score, timeLeft, totalScore, highScore, userSave);
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
        if (Time.timeScale == 0)
        {
            paused.TransitionTo(.01f);
        }
        else
        {
            unpaused.TransitionTo(.01f);
        }
    }

    private IEnumerator GameOver()
    {
        player.SetActive(false);
        gameOverUI.SetActive(true);
        
        timeLeft = time;
        totalScore = (timeLeft * 10) + score;
        SaveGame();

        yield return new WaitForSeconds(2);

        AudioController.Instance.StopMusic("Jump!");
        AudioController.Instance.StopMusic("Crack The Code");
        AudioController.Instance.MuteMusic("Elucidate", false);

        LoadNextScene("EndlessSelesai");
    }
    
    #endregion
}
