using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{

    //list of filtered Resolution
    List<Resolution> filteredRes;

    bool fullScreen = true;

    [Header("Pengaturan Window")]
    public GameObject pengaturanWindow;

    [Header("Tentang Window")]
    public GameObject tentangWindow;
    public GameObject containerTentang;
    public GameObject containerCredits;
    public Button tutupButton;
    public Button creditsButton;

    [Header("Pengaturan Object")]
    public Toggle fullScreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public AudioMixer masterMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider effectVolumeSlider;

    [Header("Pengaturan Button")]
    public Button tentangButton;
    public Button simpanButton;
    public Button batalButton;

    #region Unity Messages

    void OnEnable()
    {
        batalButton.onClick.AddListener(delegate { CancelSettings(); });
        simpanButton.onClick.AddListener(delegate { ApplySettings(); });
        tentangButton.onClick.AddListener(delegate { Tentang(); });

        tutupButton.onClick.AddListener(delegate { Tentang(); });
        creditsButton.onClick.AddListener(delegate { Credits(); });

        fullScreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(fullScreenToggle.isOn); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("masterVolume", masterVolumeSlider.value); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("musicVolume", musicVolumeSlider.value); });
        effectVolumeSlider.onValueChanged.AddListener(delegate { SetVolume("effectVolume", effectVolumeSlider.value); });
    }

    IEnumerator Start()
    {
        //Wait for Game Settings
        while (!GameSettings.Instance.IsReady)
        {
            yield return null;
        }

        var user = GameSettings.Instance.UserSettings;

        //Master Volume
        masterVolumeSlider.value = user.masterVolume;
        musicVolumeSlider.value = user.musicVolume;
        effectVolumeSlider.value = user.effectVolume;

        //Populate Resolution Dropdown
        List<string> resolution = new List<string>();
        filteredRes = new List<Resolution>();

        int lw = -1;
        int lh = -1;

        int index = 0;
        int currentResIndex = -1;

        int maxWidth = 1280;
        int maxHeight = 720;

        foreach (var res in GameSettings.Instance.Resolutions)
        {
            if (res.width >= maxWidth || res.height >= maxHeight)
            {
                string formatData = string.Format("{0} x {1} @{2}hz", res.width, res.height, res.refreshRate);
                resolution.Add(formatData);
            }

            lw = res.width;
            lh = res.height;

            //check the current res.
            if (lw == user.width && lh == user.height)
            {
                currentResIndex = index;
            }

            //Add filtered res. to list
            filteredRes.Add(res);

            index++;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolution);
        resolutionDropdown.value = currentResIndex;

        fullScreenToggle.isOn = user.fullScreen;
    }

    #endregion

    #region Menu Methods

    private void Tentang()
    {
        AudioController.Instance.PlayFX("Click");
        if (!tentangWindow.activeSelf)
        {
            tentangWindow.SetActive(true);
            containerTentang.SetActive(true);
            containerCredits.SetActive(false);
        }
        else
        {
            tentangWindow.SetActive(false);
        }
    }

    private void ApplySettings()
    {
        AudioController.Instance.PlayFX("Click");
        Resolution res = filteredRes[resolutionDropdown.value];
        float masterVolume = masterVolumeSlider.value;
        float musicVolume = musicVolumeSlider.value;
        float effectVolume = effectVolumeSlider.value;

        GameSettings.Instance.SaveSettings(res.width, res.height, fullScreen, 
            masterVolume, musicVolume, effectVolume);

        pengaturanWindow.SetActive(false);
    }

    private void CancelSettings()
    {
        AudioController.Instance.PlayFX("Click");
        if (pengaturanWindow.activeSelf)
        {
            pengaturanWindow.SetActive(false);
        }
    }

    private void Credits()
    {
        if(containerTentang.activeSelf)
        {
            containerTentang.SetActive(false);
            containerCredits.SetActive(true);
        }
        else
        {
            containerTentang.SetActive(true);
            containerCredits.SetActive(false);

        }
    }

    #endregion

    #region Private Methods

    private void SetVolume(string name, float volume)
    {
        masterMixer.SetFloat(name, volume);
    }

    private void SetFullscreen(bool newVal)
    {
        fullScreen = newVal;
    }

    #endregion
}
