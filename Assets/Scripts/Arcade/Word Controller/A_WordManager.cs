using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_WordManager : MonoBehaviour {

    [Header("Word Spawner Components")]
    public GameObject wordPrefab;
    public Transform wordCanvas;
    public float wordDelay = 5f;

    [Header("Words")]
    public List<A_WordController> words;
    public A_WordController activeWord;
    public bool hasActiveWord;
    
    private ArcadeManager arcadeManager;

    private float nextWordTime = 0f;

    private void Awake()
    {
        arcadeManager = GetComponent<ArcadeManager>();
    }

    void Update()
    {
        foreach (char letter in Input.inputString.ToLower())
        {
            TypeLetter(letter);
        }

        //Word Timer
        if (Time.time >= nextWordTime)
        {
            AddWord();
            nextWordTime = Time.time + wordDelay;
            wordDelay *= .99f;
        }
    }

    public void AddWord()
    {
        A_WordController word = new A_WordController(WordGenerator.GetRandomWord(), SpawnWord());
        words.Add(word);
    }

    public void TypeLetter(char letter)
    {
        if (hasActiveWord)
        {
            if (activeWord.GetNextLetter() == letter)
            {
                AudioController.Instance.PlayFX("Type");
                activeWord.TypeLetter();;
            }
        }
        else
        {
            foreach(A_WordController word in words)
            {
                if(word.GetNextLetter() == letter)
                {
                    AudioController.Instance.PlayFX("Type");
                    activeWord = word;
                    hasActiveWord = true;
                    word.TypeLetter();
                    break;
                }
            }
        }

        if(hasActiveWord && activeWord.WordTyped())
        {
            hasActiveWord = false;
            words.Remove(activeWord);
            arcadeManager.score += 50;
        }
    }

    #region Spawn Word

    public A_WordDisplay SpawnWord()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 8f);

        GameObject wordObj = Instantiate(wordPrefab, randomPos, Quaternion.identity, wordCanvas);
        A_WordDisplay wordDisplay = wordObj.GetComponent<A_WordDisplay>();
        wordDisplay.fallSpeed += 1f * Time.deltaTime;

        return wordDisplay;
    }

    #endregion
}
