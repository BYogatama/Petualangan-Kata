using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoPCG : MonoBehaviour {

    public TMP_Text seedsVal;
    public LevelGeneration levelGen;

    private IEnumerator Start()
    {
        while (!levelGen.LevelCreationFinished)
        {
            yield return null;
        }

        seedsVal.text = "Seeds : \n";

        foreach (int seed in levelGen.levelSeeds)
        {
            seedsVal.text += " " + seed;
        }
    }

}
