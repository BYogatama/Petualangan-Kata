using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ArcadeScoreboard : SceneLoader {

    public Button playAgain;
    public Button exitToMain;

    public TMP_Text highScoreText;
    public TMP_Text newScoreText;
    public TMP_Text timeSurvivedText;

    int highScore;
    int newScore;
    int timeSurvied;    

    void OnEnable()
    {
        playAgain.onClick.AddListener(delegate { PlayAgain(); });
        exitToMain.onClick.AddListener(delegate { ExitToMain(); });
    }

	IEnumerator Start ()
    {
        while (!GameSave.Instance.IsReady)
        {
            yield return null;
        }

        AudioController.Instance.StopMusic("Jump!");
        AudioController.Instance.MuteMusic("Elucidate", false);

        Scoreboard scoreboard = gameObject.GetComponent<Scoreboard>();

        scoreboard.DisplayArcadeScore();

        LoadSave();
        highScoreText.text = highScore.ToString();
        newScoreText.text  = newScore.ToString();
        timeSurvivedText.text = timeSurvied.ToString() + " s";
	}

    void LoadSave()
    {
        newScore = GameSave.Instance.UserSave.lastArcadeScore;
        timeSurvied = GameSave.Instance.UserSave.lastArcadeTime;
        highScore = GameSave.Instance.UserSave.arcadeHighScore;
    }


    void PlayAgain()
    {
        LoadNextScene("Arcade");
    }

    void ExitToMain()
    {
        LoadNextScene("MenuUtama");
    }
}
