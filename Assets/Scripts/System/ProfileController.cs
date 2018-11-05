using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ProfileController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    ProfileManager pM;

    private void Awake()
    {
        pM = FindObjectOfType<ProfileManager>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        pM.profileSelected = true;
        pM.selectedGameObject = gameObject.transform.GetChild(0).gameObject;
        pM.selectedProfile = pM.selectedGameObject.GetComponent<TMP_Text>();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        pM.profileSelected = false;
    }
}
