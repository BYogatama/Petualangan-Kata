using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ProfileManager : SceneLoader {

    [Header("Profile Listing")]
    public GameObject profileListBox;
    public GameObject profileListView;
    public GameObject profileText;
    public Button btnBuatProfile;
    public Button btnHapusProfile;
    public Button btnPilihProfile;

    [Header("Buat Profile")]
    public GameObject warningProfile;
    public GameObject buatProfileBox;
    public TMP_InputField inputFieldProfile;
    public Button btnSimpanProfile;
    public Button btnBatal;

    [Header("Others")]
    public bool profileSelected;
    public GameObject selectedGameObject;
    public TMP_Text selectedProfile;

    List<string> profiles;
    List<string> displayedProfile;
        
    private void OnEnable()
    {
        btnBuatProfile.onClick.AddListener(delegate { BuatProfile(); });
        btnPilihProfile.onClick.AddListener(delegate { PilihProfile(); });
        btnHapusProfile.onClick.AddListener(delegate { HapusProfile(); });
        btnSimpanProfile.onClick.AddListener(delegate { SimpanProfile(); });
        btnBatal.onClick.AddListener(delegate { BatalBuat(); });
    }

    private void Start()
    {
        profiles = GameSave.Instance.UserProfile.ProfilName;
        displayedProfile = new List<string>();

        AudioController.Instance.PlayMusic("Elucidate");
        
        UpdateProfile();
    }

    void BuatProfile()
    {
        AudioController.Instance.PlayFX("Click");
        inputFieldProfile.text = "";
        profileListBox.SetActive(false);
        buatProfileBox.SetActive(true);
    }

    void BatalBuat()
    {
        AudioController.Instance.PlayFX("Click");
        profileListBox.SetActive(true);
        buatProfileBox.SetActive(false);
        UpdateProfile();
    }

    void SimpanProfile()
    {
        AudioController.Instance.PlayFX("Click");
        while (!profiles.Contains(inputFieldProfile.text))
        {
            profiles.Add(inputFieldProfile.text);
            GameSave.Instance.SaveProfile(profiles);

            profileListBox.SetActive(true);
            buatProfileBox.SetActive(false);
            UpdateProfile();
        }
    }

    void UpdateProfile()
    {
        if (profiles.Count == 0)
        {
            warningProfile.SetActive(true);
            profiles.Clear();
            BuatProfile();
        }
        else
        {
            foreach(string name in profiles)
            {
                while (!displayedProfile.Contains(name))
                {
                    displayedProfile.Add(name);
                    GameObject profile = Instantiate(profileText, profileListView.transform);
                    TMP_Text text = profile.GetComponentInChildren<TMP_Text>();
                    text.text = name;
                }
            }
        }

    }

    void PilihProfile()
    {
        GameSave.Instance.PROFILE_NAME = selectedProfile.text;
        GameSave.Instance.LoadSaveGame(selectedProfile.text);
        AudioController.Instance.PlayFX("Click");
        LoadNextScene("MenuUtama");
    }

    void HapusProfile()
    {
        AudioController.Instance.PlayFX("Click");
        if (profiles.Contains(selectedProfile.text))
        {
            profiles.Remove(selectedProfile.text);
            displayedProfile.Remove(selectedProfile.text);
            GameSave.Instance.SaveProfile(profiles);

            Destroy(selectedGameObject.transform.parent.gameObject);
            UpdateProfile();
        }
    }
}
