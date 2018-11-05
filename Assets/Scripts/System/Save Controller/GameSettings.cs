using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSettings : MonoBehaviour
{

    #region Unity Singleton pattern

    private static GameSettings _instance;

    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameSettings>();
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject("GameSettings");
                    _instance = gameObject.AddComponent<GameSettings>();
                    DontDestroyOnLoad(gameObject);
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Static names

    static readonly string SETTING_FILE = "settings";

    #endregion

    #region Public properties

    public bool IsReady { get; private set; }

    //Unity Resolutions
    public List<Resolution> Resolutions { get; private set; }
    //User GameSettings
    public UserGameSettings UserSettings { get; private set; }

    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float EffectVolume { get; private set; }

    #endregion

    #region Public methods
    
    //Save Settings.
    public void SaveSettings(int width, int height, bool fullscreen, float maVol, float muVol, float effVol)
    {
        var settings = new UserGameSettings()
        {
            fullScreen = fullscreen,
            width = width,
            height = height,
            masterVolume = maVol,
            musicVolume = muVol,
            effectVolume = effVol
        };

        string fullPath = Path.Combine(Application.persistentDataPath, SETTING_FILE);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        File.WriteAllText(fullPath, JsonUtility.ToJson(settings, true));
        ApplySettings(settings);
        UserSettings = settings;
    }

    #endregion

    #region Unity messages

    void Awake()
    {
        MasterVolume = 0;
        MusicVolume = -20;
        EffectVolume = -10;

        Resolutions = new List<Resolution>(Screen.resolutions);
        UserSettings = LoadSettings();

        IsReady = true;
    }

    #endregion

    #region Private helper methods

    UserGameSettings LoadSettings()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, SETTING_FILE);

        if(File.Exists(fullPath))
        {
            string settingsFile = File.ReadAllText(fullPath);
            var settings = JsonUtility.FromJson<UserGameSettings>(settingsFile);

            ApplySettings(settings);
            return (settings);
        }
        else
        {
            return new UserGameSettings()
            {
                fullScreen = Screen.fullScreen,
                width = Screen.width,
                height = Screen.height,
                masterVolume = MasterVolume,
                musicVolume = MusicVolume,
                effectVolume = EffectVolume
            };
        }
    }

    private void ApplySettings(UserGameSettings settings)
    {
        Screen.SetResolution(settings.width, settings.height, settings.fullScreen);
        MasterVolume = settings.masterVolume;
        MusicVolume = settings.musicVolume;
        EffectVolume = settings.effectVolume;
    }

    #endregion

}
