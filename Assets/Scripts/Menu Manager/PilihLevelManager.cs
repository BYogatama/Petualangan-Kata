using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PilihLevelManager : SceneLoader {

    [Header("Level")]
    public Button padangRumput;
    public Button gurun;
    public Button pegunungan;

    [Header("Menu Button")]
    public Button backButton;
    
    void OnEnable()
    {
        padangRumput.onClick.AddListener(delegate { OpenLevel("PadangRumput"); });
        gurun.onClick.AddListener(delegate { OpenLevel("Gurun"); });
        pegunungan.onClick.AddListener(delegate { OpenLevel("Pegunungan"); });


        backButton.onClick.AddListener(delegate { BackToMenu(); });

    }
    
    void BackToMenu()
    {
        AudioController.Instance.PlayFX("Click");
        LoadNextScene("MenuUtama");
    }

    void OpenLevel(string area)
    {
        AudioController.Instance.PlayFX("Click");
        LoadNextScene(area);
    }
}
