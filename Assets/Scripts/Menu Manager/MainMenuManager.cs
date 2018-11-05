using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : SceneLoader {

    [Header("")]
    public TMP_Text profile;

    [Header("Main Menu Button")]
    public Button mainButton;
    public Button bantuanButton;
    public Button pengaturanButton;
    public Button profilButton;
    public Button exitButton;

    [Header("Pilih Mode Button")]
    public Button arkadeButton;
    public Button endlessButton;

    [Header("Bantuan Button")]
    public Button tutupButton;
    public Button lanjutButton;
    public Button kembaliButton;
    public Image containerItem;
    public List<Sprite> itemList = new List<Sprite>();
    private int itemSpot = 0;

    [Header("Confirmation Button")]
    public Button yaButton;
    public Button tidakButton;

    [Header("Main Menu Pop-Up Window")]
    public GameObject pilihModePopUp;
    public GameObject pengaturanWindow;
    public GameObject bantuanWindow;
    public GameObject confirmationWindow;

    void OnEnable()
    {
        //Main Menu
        mainButton.onClick.AddListener(delegate { PilihMode(); });
        bantuanButton.onClick.AddListener(delegate { Bantuan(); });
        pengaturanButton.onClick.AddListener(delegate { Pengaturan(); });
        profilButton.onClick.AddListener(delegate { GantiProfil(); });
        exitButton.onClick.AddListener(delegate { Exit(); });

        //Pilih Mode
        arkadeButton.onClick.AddListener(delegate { Arkade(); });
        endlessButton.onClick.AddListener(delegate { Endless(); });

        //Bantuan
        tutupButton.onClick.AddListener(delegate { Tutup(); });
        lanjutButton.onClick.AddListener(delegate { Lanjut(); });
        kembaliButton.onClick.AddListener(delegate { Kembali(); });

        //Confirmation
        yaButton.onClick.AddListener(delegate { ConfirmYes(); });
        tidakButton.onClick.AddListener(delegate { ConfirmNo(); });

    }

    IEnumerator Start ()
    {

        while (!GameSave.Instance.IsReady)
        {
            yield return null;
        }

        profile.text = GameSave.Instance.PROFILE_NAME;

        pilihModePopUp.SetActive(false);
        pengaturanWindow.SetActive(false);
        bantuanWindow.SetActive(false);

    }

    void PilihMode()
    {
        AudioController.Instance.PlayFX("Click");
        if (!pilihModePopUp.activeSelf)
        {
            pilihModePopUp.SetActive(true);
        }
        else
        {
            pilihModePopUp.SetActive(false);
        }

    }

    void Pengaturan()
    {
        AudioController.Instance.PlayFX("Click");
        pengaturanWindow.SetActive(true);
        pilihModePopUp.SetActive(false);
    }

    void Bantuan()
    {
        AudioController.Instance.PlayFX("Click");
        bantuanWindow.SetActive(true);
        pilihModePopUp.SetActive(false);
    }
    
    private void GantiProfil()
    {
        AudioController.Instance.PlayFX("Click");
        LoadNextScene("MenuProfile");
    }

    void Exit()
    {
        AudioController.Instance.PlayFX("Click");
        confirmationWindow.SetActive(true);
        pilihModePopUp.SetActive(false);
    }

    //Pilih Mode
    void Arkade()
    {
        AudioController.Instance.PlayFX("Click");
        LoadNextScene("Arcade");
    }

    void Endless()
    {
        AudioController.Instance.PlayFX("Click");
        LoadNextScene("PilihLevel");
    }


    //Confirmation Window
    void ConfirmYes()
    {
        AudioController.Instance.PlayFX("Click");
        confirmationWindow.SetActive(false);
        Application.Quit();
    }

    void ConfirmNo()
    {
        AudioController.Instance.PlayFX("Click");
        confirmationWindow.SetActive(false);
    }

    //Bantuan Window
    void Lanjut()
    {
        AudioController.Instance.PlayFX("Click");
        if(itemSpot < itemList.Count - 1)
        {
            itemSpot++;
            containerItem.sprite = itemList[itemSpot];
        }

    }

    void Kembali()
    {
        AudioController.Instance.PlayFX("Click");
        if (itemSpot > 0)
        {
            itemSpot--;
            containerItem.sprite = itemList[itemSpot];
        }

    }

    void Tutup()
    {
        AudioController.Instance.PlayFX("Click");
        bantuanWindow.SetActive(false);
    }
}
