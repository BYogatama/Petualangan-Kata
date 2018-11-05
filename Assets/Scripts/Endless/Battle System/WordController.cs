using UnityEngine;

[System.Serializable]
public class WordController {

    public string word;

    public int typeIndex;

    WordDisplay display;

    public WordController(string _word, WordDisplay _display)
    {
        word = _word;
        typeIndex = 0;

        display = _display;
        display.SetWord(word);
        display.textObject.color = Color.white;
    }

    public char GetNextLetter()
    {
       return word[typeIndex];
    }

    public void TypeLetter()
    {
        typeIndex++;
        display.RemoveLetter();
    }

    public bool WordTyped()
    {
        bool wordTyped = (typeIndex >= word.Length);
        if (wordTyped)
        {
            display.RemoveWord();
        }
        return wordTyped;
    }

}
