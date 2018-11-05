using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class ArcadeScore
{
    public string Profiles { get; set; }
    public int Score { get; set; }
    public int Time { get; set; }
}

[System.Serializable]
public class EndlessScore
{
    public string Profiles { get; set; }
    public string Level { get; set; }
    public int Score { get; set; }
}

public class Scoreboard : MonoBehaviour {

    List<ArcadeScore> arcadeHighScores = new List<ArcadeScore>();
    List<EndlessScore> endlessHighScores = new List<EndlessScore>();

    List<string> profiles;

    public int maxDisplayedScore;
    public GameObject textPrefab;
    public GameObject nameHolder;
    public GameObject scoreHolder;
    public GameObject timeHolder;

    private int topScore;

    public void DisplayArcadeScore()
    {
        //Load Profile Names
        profiles = GameSave.Instance.UserProfile.ProfilName;

        //Load Save Game
        for (int i = 0; i < profiles.Count; i++)
        {
            var savegame = GameSave.Instance.LoadSaveGame(profiles[i]);
            arcadeHighScores.Add(new ArcadeScore
            {
                Profiles = profiles[i],
                Score = savegame.arcadeHighScore,
                Time = savegame.longestArcadetTime
            });
        }

        //Check for the Highest Score
        arcadeHighScores = arcadeHighScores.OrderByDescending(a => a.Score).ToList();
        topScore = arcadeHighScores.Count;

        if (topScore >= maxDisplayedScore)
        {
            topScore = maxDisplayedScore;
        }

        for (int i = 0; i < topScore; i++)
        {
            GameObject name = Instantiate(textPrefab, nameHolder.transform);
            GameObject score = Instantiate(textPrefab, scoreHolder.transform);
            GameObject time = Instantiate(textPrefab, timeHolder.transform);

            TMP_Text namaText = name.GetComponent<TMP_Text>();
            TMP_Text scoreText = score.GetComponent<TMP_Text>();
            TMP_Text timeText = time.GetComponent<TMP_Text>();

            namaText.text = arcadeHighScores[i].Profiles;
            scoreText.text = arcadeHighScores[i].Score.ToString();
            timeText.text = arcadeHighScores[i].Time.ToString() + " s";
        }
    }


    public void DisplayEndlessScore(string levelName)
    {
        //Load Profile Names
        profiles = GameSave.Instance.UserProfile.ProfilName;

        //Load Save Game
        for (int i = 0; i < profiles.Count; i++)
        {
            var savegame = GameSave.Instance.LoadSaveGame(profiles[i]);
            
            if (levelName == "Padang Rumput")
            {
                endlessHighScores.Add(new EndlessScore
                {
                    Profiles = profiles[i],
                    Score = savegame.pr_highScore,
                    Level = levelName
                });
            }
            else if(levelName == "Pegunungan")
            {
                endlessHighScores.Add(new EndlessScore
                {
                    Profiles = profiles[i],
                    Score = savegame.pe_highScore,
                    Level = levelName
                });

            }
            else if (levelName == "Gurun")
            {
                endlessHighScores.Add(new EndlessScore
                {
                    Profiles = profiles[i],
                    Score = savegame.gr_highScore,
                    Level = levelName
                });

            }
        }

        //Check for the Highest Score
        endlessHighScores = endlessHighScores.OrderByDescending(a => a.Score).ToList();
        topScore = endlessHighScores.Count;

        if (topScore >= maxDisplayedScore)
        {
            topScore = maxDisplayedScore;
        }

        for (int i = 0; i < topScore; i++)
        {
            GameObject name = Instantiate(textPrefab, nameHolder.transform);
            GameObject score = Instantiate(textPrefab, scoreHolder.transform);
            GameObject level = Instantiate(textPrefab, timeHolder.transform);

            TMP_Text namaText = name.GetComponent<TMP_Text>();
            TMP_Text scoreText = score.GetComponent<TMP_Text>();
            TMP_Text levelText = level.GetComponent<TMP_Text>();

            namaText.text = endlessHighScores[i].Profiles;
            scoreText.text = endlessHighScores[i].Score.ToString();
            levelText.text = endlessHighScores[i].Level;
        }
    }
}
