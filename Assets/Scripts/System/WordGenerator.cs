using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class WordGenerator : MonoBehaviour {

    public static List<string> wordList = new List<string>();

    public static string GetRandomWord()
    {
        string filePath = Application.streamingAssetsPath + "/ListKata.txt";
        if (File.Exists(filePath))
        {
            var text = File.ReadAllText(filePath);
            var punctuation = text.Where(char.IsPunctuation).Distinct().ToArray();
            var words = text.Split().Select(x => x.Trim(punctuation));

            foreach (string word in words)
            {
                if (isAlphabets(word))
                {
                    if (wordList.Contains(word))
                    {
                        continue;
                    }

                    wordList.Add(word.ToLower());
                }
            }
        }
        else
        {
            var assets = Resources.Load<TextAsset>("Word/WordList");
            var punctuation = assets.text.Where(char.IsPunctuation).Distinct().ToArray();
            var words = assets.text.Split().Select(x => x.Trim(punctuation));

            foreach (string word in words)
            {
                if (isAlphabets(word))
                {
                    if (wordList.Contains(word))
                    {
                        continue;
                    }

                    wordList.Add(word.ToLower());
                }
            }
        }


        //File.WriteAllLines(Application.streamingAssetsPath + "/Words/Wordist.txt", wordList.ToArray());

        int randomIndex = Random.Range(0, wordList.Count);
        string randomWord = wordList[randomIndex];

        return randomWord;
    }

    public static bool isAlphabets(string strToCheck)
    {
        Regex rg = new Regex(@"^[a-zA-Z,]*$");
        return rg.IsMatch(strToCheck);
    }

}
