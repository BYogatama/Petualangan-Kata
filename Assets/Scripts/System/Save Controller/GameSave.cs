using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSave : MonoBehaviour {

    #region Unity Singleton pattern

    private static GameSave _instance;

    public static GameSave Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameSave>();
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject("GameSave");
                    _instance = gameObject.AddComponent<GameSave>();
                    DontDestroyOnLoad(gameObject);
                }
            }

            return _instance;
        }
    }

    #endregion

    #region Static names

    static readonly string SAVE_FILE = "savegame";
    static readonly string PROFILES_FILE = "profiles";

    #endregion

    #region Public properties

    public bool IsReady { get; private set; }

    public UserGameSave UserSave { get; private set; }
    public UserGameProfile UserProfile { get; private set; }

    public string PROFILE_NAME = "";
    public string LEVEL_NAME = "";

    #endregion

    #region Public methods

    public void SaveProfile(List<string> profileName)
    {
        var saveprofile = new UserGameProfile()
        {
            ProfilName = profileName
        };

        string fullPath = Path.Combine(Application.persistentDataPath, PROFILES_FILE);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        File.WriteAllText(fullPath, JsonUtility.ToJson(saveprofile, true));
        UserProfile = saveprofile;
    }

    //
    public void SaveArcadeGame(int high_score_a, int cur_score_a, int long_time_a, int cur_time_a, UserGameSave old_save)
    {
        var savegame = new UserGameSave()
        {
            arcadeHighScore = high_score_a,
            lastArcadeScore = cur_score_a,
            longestArcadetTime = long_time_a,
            lastArcadeTime = cur_time_a,

            //Old One
            pr_lastScore = old_save.pr_lastScore,
            pr_lastTimeLeft = old_save.pr_lastTimeLeft,
            pr_lastTotalScore = old_save.pr_lastTotalScore,
            pr_highScore = old_save.pr_highScore,

            //Old One
            pe_lastScore = old_save.pe_lastScore,
            pe_lastTimeLeft = old_save.pe_lastTimeLeft,
            pe_lastTotalScore = old_save.pe_lastTotalScore,
            pe_highScore = old_save.pe_highScore,

            //Old One
            gr_lastScore = old_save.gr_lastScore,
            gr_lastTimeLeft = old_save.gr_lastTimeLeft,
            gr_lastTotalScore = old_save.gr_lastTotalScore,
            gr_highScore = old_save.gr_highScore
        };

        string fullPath = Path.Combine(Application.persistentDataPath, SAVE_FILE + "-" + PROFILE_NAME);

        /*if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }*/

        File.WriteAllText(fullPath, JsonUtility.ToJson(savegame, true));
        UserSave = savegame;
    }

    //
    public void SaveEndlessGame(string levelName, int lastScore, int lastTimeLeft, int lastTotalScore, int highScore, UserGameSave old_save)
    {
        var savegame = new UserGameSave();
        
        if (levelName == "Padang Rumput")
        {
            savegame = new UserGameSave()
            {
                //Old One
                arcadeHighScore = old_save.arcadeHighScore,
                longestArcadetTime = old_save.longestArcadetTime,
                lastArcadeScore = old_save.lastArcadeScore,
                lastArcadeTime = old_save.lastArcadeTime,

                //New Save
                pr_lastScore = lastScore,
                pr_lastTimeLeft = lastTimeLeft,
                pr_lastTotalScore = lastTotalScore,
                pr_highScore = highScore,
                
                //Old One
                pe_lastScore = old_save.pe_lastScore,
                pe_lastTimeLeft = old_save.pe_lastTimeLeft,
                pe_lastTotalScore = old_save.pe_lastTotalScore,
                pe_highScore = old_save.pe_highScore,
                
                //Old One
                gr_lastScore = old_save.gr_lastScore,
                gr_lastTimeLeft = old_save.gr_lastTimeLeft,
                gr_lastTotalScore = old_save.gr_lastTotalScore,
                gr_highScore = old_save.gr_highScore

            };
        }
        else if (levelName == "Pegunungan")
        {
            savegame = new UserGameSave()
            {
                //Old One
                arcadeHighScore = old_save.arcadeHighScore,
                longestArcadetTime = old_save.longestArcadetTime,
                lastArcadeScore = old_save.lastArcadeScore,
                lastArcadeTime = old_save.lastArcadeTime,

                //Old One
                pr_lastScore = old_save.pr_lastScore,
                pr_lastTimeLeft = old_save.pr_lastTimeLeft,
                pr_lastTotalScore = old_save.pr_lastTotalScore,
                pr_highScore = old_save.pr_highScore,

                //New Save
                pe_lastScore = lastScore,
                pe_lastTimeLeft = lastTimeLeft,
                pe_lastTotalScore = lastTotalScore,
                pe_highScore = highScore,

                //Old One
                gr_lastScore = old_save.gr_lastScore,
                gr_lastTimeLeft = old_save.gr_lastTimeLeft,
                gr_lastTotalScore = old_save.gr_lastTotalScore,
                gr_highScore = old_save.gr_highScore
            };
        }
        else if (levelName == "Gurun")
        {
            savegame = new UserGameSave()
            {
                //Old One
                arcadeHighScore = old_save.arcadeHighScore,
                longestArcadetTime = old_save.longestArcadetTime,
                lastArcadeScore = old_save.lastArcadeScore,
                lastArcadeTime = old_save.lastArcadeTime,

                //Old One
                pr_lastScore = old_save.pr_lastScore,
                pr_lastTimeLeft = old_save.pr_lastTimeLeft,
                pr_lastTotalScore = old_save.pr_lastTotalScore,
                pr_highScore = old_save.pr_highScore,
                
                //Old One
                pe_lastScore = old_save.pe_lastScore,
                pe_lastTimeLeft = old_save.pe_lastTimeLeft,
                pe_lastTotalScore = old_save.pe_lastTotalScore,
                pe_highScore = old_save.pe_highScore,

                //New Save
                gr_lastScore = lastScore,
                gr_lastTimeLeft = lastTimeLeft,
                gr_lastTotalScore = lastTotalScore,
                gr_highScore = highScore
            };
        }

        string fullPath = Path.Combine(Application.persistentDataPath, SAVE_FILE + "-" + PROFILE_NAME);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        File.WriteAllText(fullPath, JsonUtility.ToJson(savegame, true));
        UserSave = savegame;
    }

    #endregion

    #region Unity messages

    void Awake()
    {
        UserProfile = LoadProfile();

        IsReady = true;
    }

    #endregion

    #region Private helper methods

    public UserGameSave LoadSaveGame(string PROFILES)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, SAVE_FILE + "-" + PROFILES);

        if (File.Exists(fullPath))
        {
            string savegameFile = File.ReadAllText(fullPath);
            var savegame = JsonUtility.FromJson<UserGameSave>(savegameFile);
            Debug.Log("File Found");

            return (savegame);
        }
        else
        {
            return new UserGameSave()
            {
                arcadeHighScore = 0,
                longestArcadetTime = 0,
                lastArcadeScore = 0,
                lastArcadeTime = 0,
                
                pr_lastScore = 0,
                pr_lastTimeLeft = 0,
                pr_lastTotalScore = 0,
                pr_highScore = 0,
                
                pe_lastScore = 0,
                pe_lastTimeLeft = 0,
                pe_lastTotalScore = 0,
                pe_highScore = 0,
                
                gr_lastScore = 0,
                gr_lastTimeLeft = 0,
                gr_lastTotalScore = 0,
                gr_highScore = 0,

            };
        }
    }

    UserGameProfile LoadProfile()
    {
        string fullPath = Path.Combine(Application.persistentDataPath, PROFILES_FILE);

        if (File.Exists(fullPath))
        {
            string profileFile = File.ReadAllText(fullPath);
            var saveprofile = JsonUtility.FromJson<UserGameProfile>(profileFile);

            return (saveprofile);
        }
        else
        {
            return new UserGameProfile()
            {
                ProfilName = new List<string>()
            };
        }
    }

    #endregion
}
