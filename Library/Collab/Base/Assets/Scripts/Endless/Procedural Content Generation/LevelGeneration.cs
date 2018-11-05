using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour {

    #region Inspector

    [Header("Procedural Content Generation")]
    public bool repeatLevelPattern;

    [Header("Generation Toggle")]
    [SerializeField]
    private bool generateLevels;
    [SerializeField]
    private bool generateEnvirontments;
    [SerializeField]
    private bool generateEnemy;

    [Header("Level Length")]
    [SerializeField]
    private int minLevel;
    [SerializeField]
    private int maxLevel;

    [Header("Checkpoints")]
    [SerializeField]
    private GameObject checkpoints;
    [SerializeField]
    private GameObject coins;

    [Header("Level Pattern")]
    [SerializeField]
    private GameObject[] entryLevels;
    [SerializeField]
    private GameObject[] theLevels;
    [SerializeField]
    private GameObject[] exitLevels;
    
    [Header("Grass-Like")]
    [SerializeField]
    private GameObject[] grassLike;

    [Header("Stone or Bush")]
    [SerializeField]
    private int minSBDensity;
    [SerializeField]
    private int maxSBDensity;
    [SerializeField]
    private GameObject[] stoneOrBush;

    [Header("Trees")]
    [SerializeField]
    private int minTreeDensity;
    [SerializeField]
    private int maxTreeDensity;
    [SerializeField]
    private GameObject[] trees;

    [Header("Enemies")]
    [SerializeField]
    private GameObject[] enemies;

    [Header("Others")]
    [SerializeField]
    private GameObject filler;


    #endregion

    #region Private Variabels

    private GameObject[] groundTop;
    private GameObject[] ground;
    private GameObject[] halfTop;
    private GameObject[] hazard;

    private AlignLevel alignLevel;

    private int[] levelSeeds;
    private int levelLength;
    private int lengthOfPattern;

    private bool EntryIsCreated;
    private bool LevelIsCreated;
    private bool ExitIsCreated;
    private bool EnvironmentsIsCreated;

    #endregion

    #region Public Variabels
    
    [HideInInspector]
    public bool LevelCreationFinished;

    #endregion

    void Start()
    {
        CreateSeeds();
        GenerateLevels();
        
        if(!LevelCreationFinished)
        {
            groundTop = GameObject.FindGameObjectsWithTag("GroundTop");
            halfTop = GameObject.FindGameObjectsWithTag("HalfTop");
            ground = GameObject.FindGameObjectsWithTag("Ground");
            hazard = GameObject.FindGameObjectsWithTag("Hazard");
            
            GenerateEnvironments();
            GenerateEnemies();
            GenerateCheckpoints();
            GeneratePickups();

            FillLevels();

            LevelCreationFinished = true;
        }
    }

    #region Procedural Content Generation

    #region Level Seeds Creation

    void CreateSeeds()
    {
        //Define for Indexing
        int indexOfEntry;
        int indexOfExit;
        int[] indexOfLevel;

        if (!repeatLevelPattern)
        {
            maxLevel = theLevels.Length;
            minLevel = maxLevel - 5;
        }

        //Randomize Level Length
        levelLength = Random.Range(minLevel, maxLevel + 1);

        //Define Length of Seed
        levelSeeds = new int[levelLength];

        //Get Random Index of Entry Levels
        indexOfEntry = Random.Range(0, entryLevels.Length);

        //Create Index of Levels
        lengthOfPattern = theLevels.Length;                         //Get Length of Level Pattern
        indexOfLevel = new int[levelLength - 2];                    //Get Total of Level Index
        List<int> usedChunk = new List<int>();                      //Define List for Storing used Level Chunk
        for (int i = 0; i < indexOfLevel.Length; i++)
        {
            if (!repeatLevelPattern)
            {
                if (i != 0)
                {
                    while (usedChunk.Contains(indexOfLevel[i]))
                    {
                        indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                    }
                    usedChunk.Add(indexOfLevel[i]);
                }
                else
                {
                    indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                    usedChunk.Add(indexOfLevel[i]);
                }
            }
            else
            {
                if (i != 0)
                {
                    indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                    while (indexOfLevel[i] == indexOfLevel[i - 1])
                    {
                        indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                    }
                }
                else
                {
                    indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                }
            }
        }
        
        //Get Random Index of Exit Levels
        indexOfExit = Random.Range(0, exitLevels.Length);

        //Populate Seed Level
        for(int i = 0; i < levelSeeds.Length; i++)
        {
            if(i == 0)
            {
                levelSeeds[i] = indexOfEntry;
            }
            else if(i == levelSeeds.Length - 1)
            {
                levelSeeds[i] = indexOfExit;
            }
            else
            {
                levelSeeds[i] = indexOfLevel[i - 1];
            }
        }

    }

    #endregion

    #region Levels Creation

    private void GenerateLevels()
    {
        if (generateLevels)
        {
            //Create Entry Level
            while (!EntryIsCreated)
            {
                Instantiate(entryLevels[levelSeeds[0]], transform.position, Quaternion.identity, transform);
                EntryIsCreated = true;
            }

            //If Entry Level Created, Create Mid Level
            while (EntryIsCreated && !LevelIsCreated)
            {
                for (int i = 1; i <= levelLength - 2; i++)
                {
                    Vector3 position = new Vector3(transform.position.x + ((i) * 10), transform.position.y, transform.position.z);
                    GameObject level = Instantiate(theLevels[levelSeeds[i]], position, Quaternion.identity, transform);

                    alignLevel = level.GetComponent<AlignLevel>();
                    alignLevel.Align();
                }

                LevelIsCreated = true;
            }

            //If Entry & Level is Ready Create Exit Level
            while (EntryIsCreated && LevelIsCreated && !ExitIsCreated)
            {
                Vector3 position = new Vector3(transform.position.x + ((levelLength - 1) * 10), transform.position.y, transform.position.z);
                GameObject exit = Instantiate(exitLevels[levelSeeds[levelLength - 1]], position, Quaternion.identity, transform);

                alignLevel = exit.GetComponent<AlignLevel>();
                alignLevel.Align();

                ExitIsCreated = true;
            }
        }
    }

    #endregion

    #region Environments Creation

    private void GenerateEnvironments()
    {
        if (generateEnvirontments)
        {
            //Create GameObject of Environment
            GameObject Environments = new GameObject("Environments");
            Environments.transform.SetParent(transform);
            
            while (!EnvironmentsIsCreated)
            {
                if (grassLike.Length > 0)
                {
                    for (int i = 0; i < groundTop.Length; i++)
                    {
                        int index = Random.Range(0, grassLike.Length);
                        Vector3 posGroundTop = new Vector3(groundTop[i].transform.position.x, groundTop[i].transform.position.y + 0.5f, groundTop[i].transform.position.z);
                        Instantiate(grassLike[index], posGroundTop, Quaternion.identity, Environments.transform);
                    }

                    for (int i = 0; i < halfTop.Length; i++)
                    {
                        int index = Random.Range(0, grassLike.Length);
                        Vector3 posHalfTop = new Vector3(halfTop[i].transform.position.x, halfTop[i].transform.position.y + 0.5f, halfTop[i].transform.position.z);
                        Instantiate(grassLike[index], posHalfTop, Quaternion.identity, Environments.transform);
                    }
                }

                if (stoneOrBush.Length > 0)
                {
                    int Rand = Random.Range(minSBDensity, maxSBDensity + 1);
                    for (int i = 0; i < groundTop.Length; i += Rand)
                    {
                        int index = Random.Range(0, stoneOrBush.Length);
                        Vector3 postition = new Vector3(groundTop[i].transform.position.x + 0.6f, groundTop[i].transform.position.y + 0.5f, groundTop[i].transform.position.z);
                        Instantiate(stoneOrBush[index], postition, Quaternion.identity, Environments.transform);
                    }
                }

                if (trees.Length > 0)
                {
                    int Rand = Random.Range(minTreeDensity, maxTreeDensity + 1);
                    for (int i = 0; i < groundTop.Length; i += Rand)
                    {
                        int index = Random.Range(0, trees.Length);
                        Vector3 postition = new Vector3(groundTop[i].transform.position.x + 0.6f, groundTop[i].transform.position.y + 0.5f, groundTop[i].transform.position.z);
                        Instantiate(trees[index], postition, Quaternion.identity, Environments.transform);
                    }
                }

                EnvironmentsIsCreated = true;
            }
        }
    }

    #endregion

    #region Enemies Creation

    private void GenerateEnemies()
    {
        if (generateEnemy)
        {
            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.SetParent(transform);
            
            if (EnvironmentsIsCreated)
            {
                for (int i = 10; i < groundTop.Length; i += 15)
                {
                    int RandomEnemy = Random.Range(0, enemies.Length);
                    Vector3 posGroundTop = new Vector3(groundTop[i].transform.position.x, groundTop[i].transform.position.y + 1.3f, groundTop[i].transform.position.z);
                    Instantiate(enemies[RandomEnemy], posGroundTop, Quaternion.identity, Enemies.transform);
                }
            }
        }

    }

    #endregion

    #endregion
    
    #region Privates Method

    private void GenerateCheckpoints()
    {
        GameObject Checkpoints = new GameObject("Checkpoints");
        Checkpoints.transform.SetParent(transform);

        for (int i = 15; i < groundTop.Length; i += 20)
        {
            Vector3 posGroundTop = new Vector3(groundTop[i].transform.position.x, groundTop[i].transform.position.y + 0.5f, groundTop[i].transform.position.z);
            Instantiate(checkpoints, posGroundTop, Quaternion.identity, Checkpoints.transform);
        }
    }

    private void GeneratePickups()
    {
        GameObject Coins = new GameObject("Coins");
        Coins.transform.SetParent(transform);
        
        for (int i = 5; i < groundTop.Length; i++)
        {
            Vector3 posGroundTop = new Vector3(groundTop[i].transform.position.x, groundTop[i].transform.position.y + 1f, groundTop[i].transform.position.z);
            Instantiate(coins, posGroundTop, Quaternion.identity, Coins.transform);
        }

        for (int i =0; i < halfTop.Length; i++)
        {
            Vector3 posHalfTop = new Vector3(halfTop[i].transform.position.x, halfTop[i].transform.position.y + 1f, halfTop[i].transform.position.z);
            Instantiate(coins, posHalfTop, Quaternion.identity, Coins.transform);
        }

    }

    private void FillLevels()
    {
        GameObject Filler = new GameObject("Filler");
        Filler.transform.SetParent(transform);

        for (int i = 0; i < groundTop.Length; i++)
        {
            Debug.DrawRay(new Vector2(groundTop[i].transform.position.x, groundTop[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(groundTop[i].transform.position.x, groundTop[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if (hit == false)
            {
                for (int j = 1; j <= 20; j++)
                {
                    Instantiate(filler, new Vector3(groundTop[i].transform.position.x, groundTop[i].transform.position.y - j,
                        groundTop[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }

        }

        for (int i = 0; i < ground.Length; i++)
        {
            Debug.DrawRay(new Vector2(ground[i].transform.position.x, ground[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(ground[i].transform.position.x, ground[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if (hit == false)
            {
                for (int j = 1; j <= 19; j++)
                {
                    Instantiate(filler, new Vector3(ground[i].transform.position.x, ground[i].transform.position.y - j,
                        ground[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }
        }

        for (int i = 0; i < hazard.Length; i++)
        {
            Debug.DrawRay(new Vector2(hazard[i].transform.position.x, hazard[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(hazard[i].transform.position.x, hazard[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if (hit == false)
            {
                for (int j = 1; j <= 20; j++)
                {
                    Instantiate(filler, new Vector3(hazard[i].transform.position.x, hazard[i].transform.position.y - j,
                        hazard[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }
        }

    }

    #endregion
}