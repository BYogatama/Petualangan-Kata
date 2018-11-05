using UnityEngine;
using TMPro;

public class WordDisplay : MonoBehaviour {
    
    public TMP_Text textObject;

    public void SetWord(string word)
    {
        textObject.text = word.ToUpper();
    }

    public void RemoveLetter()
    {
        textObject.text = textObject.text.Remove(0, 1);
        textObject.color = Color.red;
    }

    public void RemoveWord()
    {
        textObject.text = "---";
        textObject.color = Color.white;
    }
}
