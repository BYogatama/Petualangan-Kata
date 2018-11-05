using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndlessScoreboard : SceneLoader {
    
    [Header("Scoreboard")]
    public TMP_Text scoreText;
    public TMP_Text timeLeftText;
    public TMP_Text totalScoreText;

    public Button keluarBtn;
    public Button ulangiBtn;
    public Button lanjutBtn;

    int score;
    int timeLeft;
    int totalScore;

    string levelName;

    private void OnEnable()
    {
        keluarBtn.onClick.AddListener(delegate { LoadNextScene("MenuUtama"); });
        ulangiBtn.onClick.AddListener(delegate { LoadLastScene(); });
        lanjutBtn.onClick.AddListener(delegate { LoadNextScene("PilihLevel"); });
    }

    private void Start () {
        levelName = GameSave.Instance.LEVEL_NAME;
        Scoreboard scoreboard = gameObject.GetComponent<Scoreboard>();
        scoreboard.DisplayEndlessScore(levelName);

        LoadSave();
        scoreText.text = score.ToString();
        timeLeftText.text = timeLeft.ToString() + " s";
        totalScoreText.text = totalScore.ToString();
    }

    private void LoadLastScene()
    {
        var savegame = GameSave.Instance.UserSave;
        if (levelName == "Padang Rumput")
        {
            LoadNextScene("PadangRumput");
        }
        else if (levelName == "Pegunungan")
        {
            LoadNextScene("Pegunungan");
        }
        else if (levelName == "Gurun")
        {
            LoadNextScene("Gurun");
        }
    }
    
    private void LoadSave()
    {
        var savegame = GameSave.Instance.UserSave;
        if(levelName == "Padang Rumput")
        {
            score = savegame.pr_lastScore;
            timeLeft = savegame.pr_lastTimeLeft;
            totalScore = savegame.pr_lastTotalScore;
        }
        else if (levelName == "Pegunungan")
        {
            score = savegame.pe_lastScore;
            timeLeft = savegame.pe_lastTimeLeft;
            totalScore = savegame.pe_lastTotalScore;
        }
        else if (levelName == "Gurun")
        {
            score = savegame.gr_lastScore;
            timeLeft = savegame.gr_lastTimeLeft;
            totalScore = savegame.gr_lastTotalScore;
        }
    }
}
