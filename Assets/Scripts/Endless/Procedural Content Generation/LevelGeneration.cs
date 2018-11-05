using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    #region Inspector

    [Header("Procedural Content Generation")]
    [Space]
    public bool allowRepeatLevelPattern;
    public GameObject player;

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
    private int enemyDensity;
    [SerializeField]
    private GameObject[] enemies;

    [Header("Others")]
    [SerializeField]
    private GameObject filler;


    #endregion

    #region Private Variabels

    private GameObject[] grndTop;
    private GameObject[] grounds;
    private GameObject[] halfTop;
    private GameObject[] hazards;

    private int levelLength;
    private int lengthOfPattern;

    private bool EntryIsCreated = false;
    private bool LevelIsCreated = false;
    private bool ExitIsCreated = false;

    private bool levelCreation;
    private bool environmentCreation;
    private bool enemiesCreation;

    #endregion

    #region Public Variabels

    [HideInInspector]
    public int[] levelSeeds;
    [HideInInspector]
    public bool LevelCreationFinished;

    #endregion

    void Start()
    {
        CreateSeeds();
        if (generateLevels)
        {
            GenerateLevels();
        }

        if (!levelCreation)
        {
            grndTop = GameObject.FindGameObjectsWithTag("GroundTop");
            halfTop = GameObject.FindGameObjectsWithTag("HalfTop");
            grounds = GameObject.FindGameObjectsWithTag("Ground");
            hazards = GameObject.FindGameObjectsWithTag("Hazard");

            if (generateEnvirontments)
            {
                GenerateEnvironments();
            }

            if (generateEnemy)
            {
                GenerateEnemies();
            }

            GenerateCheckpoints();
            GeneratePickups();

            FillLevels();

            LevelCreationFinished = true;
        }
    }

    #region Procedural Content Generation

    #region Level Seeds Creation

    private void CreateSeeds()
    {
        // # Define for Indexing
        int indexOfEntry;
        int indexOfExit;
        int[] indexOfLevel;

        // # If Pattern not allowed to Repeat Set Max Level and Min Level
        if (!allowRepeatLevelPattern)
        {
            maxLevel = theLevels.Length + 2;
            minLevel = maxLevel;
        }

        // # Randomize Level Length
        levelLength = Random.Range(minLevel, maxLevel + 1);

        // # Define Length of Level Seed
        levelSeeds = new int[levelLength];

        // # Set Entry Index
        indexOfEntry = Random.Range(0, entryLevels.Length);

        // # Set Index of Levels
        // # Set Length of Level Pattern = theLevels Length
        lengthOfPattern = theLevels.Length;

        // # Set Index of Level = Length of Level - 2 (Entry & Exit)
        indexOfLevel = new int[levelLength - 2];

        // # Define List for Storing Used Pattern
        List<int> usedPattern = new List<int>();

        for (int i = 0; i < indexOfLevel.Length; i++)
        {
            if (!allowRepeatLevelPattern)
            {
                if (i != 0)
                {
                    while (usedPattern.Contains(indexOfLevel[i]))
                    {
                        indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                    }
                    usedPattern.Add(indexOfLevel[i]);
                }
                else
                {
                    indexOfLevel[i] = Random.Range(0, lengthOfPattern);
                    usedPattern.Add(indexOfLevel[i]);
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

        // # Set Index of Exit Levels
        indexOfExit = Random.Range(0, exitLevels.Length);

        /* 
         * Populate Level Seeds
         */
        for (int i = 0; i < levelSeeds.Length; i++)
        {
            if (i == 0)
            {
                levelSeeds[i] = indexOfEntry;                                       // First Executed
            }
            else if (i == levelSeeds.Length - 1)
            {
                levelSeeds[i] = indexOfExit;                                        // Last Executed
            }
            else
            {
                levelSeeds[i] = indexOfLevel[i - 1];
            }
        }

        levelCreation = true;

    }

    #endregion

    #region Levels Creation

    private void GenerateLevels()
    {
        if (levelCreation)
        {
            GameObject levelsParent = new GameObject("Levels");
            levelsParent.transform.SetParent(transform);

            AlignLevel alignLevel;

            int lastPatternSize = 0;

            EntryIsCreated = false;
            LevelIsCreated = false;
            ExitIsCreated = false;

            Vector3 lastPosition = new Vector3(0, 0, 0);

            // # Create Entry Level & Spawn Player
            while (!EntryIsCreated)
            {
                // # Instansiate Entry Level
                GameObject entry = Instantiate(entryLevels[levelSeeds[0]], transform.position, Quaternion.identity, levelsParent.transform);

                // # Get Entry Level Pattern Size & Store It In lastPatternSize
                alignLevel = entry.GetComponent<AlignLevel>();
                lastPatternSize = alignLevel.patternSize;

                // # Get PlayerSPawn Transform
                Transform playerSpawn = entry.transform.Find("PlayerSpawn");

                // # Instansiate Player to Player Spawn Position & Destroy PlayerSpawn GameObject
                Instantiate(player, playerSpawn.position, Quaternion.identity, transform.parent);
                Destroy(playerSpawn.gameObject);

                // # Store Entry Level Position
                lastPosition = entry.transform.position;

                EntryIsCreated = true;
            }

            // # Create Levels & Align It
            while (EntryIsCreated && !LevelIsCreated)
            {
                GameObject[] level = new GameObject[levelLength - 2];

                for (int i = 1; i <= levelLength - 2; i++)
                {
                    // # Set Position of Levels
                    Vector3 position = new Vector3(lastPosition.x + lastPatternSize, transform.position.y, transform.position.z);

                    // # Instansiate Levels at Defined Position
                    level[i - 1] = Instantiate(theLevels[levelSeeds[i]], position, Quaternion.identity, levelsParent.transform);

                    // # Store Instansiated Object Position
                    lastPosition = level[i - 1].transform.position;

                    // # Store Pattern Size in lastPatternSize & Align Levels
                    alignLevel = level[i - 1].GetComponent<AlignLevel>();
                    lastPatternSize = alignLevel.patternSize;
                    alignLevel.Align();
                }

                LevelIsCreated = true;
            }

            // # Create Exit Levels
            while (EntryIsCreated && LevelIsCreated && !ExitIsCreated)
            {
                // # Set Position of Exits Levels
                Vector3 position = new Vector3(lastPosition.x + lastPatternSize, transform.position.y, transform.position.z);

                // # Instansiate Exit Levels at Position
                GameObject exit = Instantiate(exitLevels[levelSeeds[levelLength - 1]], position, Quaternion.identity, levelsParent.transform);

                // # Align Exit Levels to Last Levels
                alignLevel = exit.GetComponent<AlignLevel>();
                alignLevel.Align();

                ExitIsCreated = true;
            }

            levelCreation = false;
            environmentCreation = true;
        }
    }

    #endregion

    #region Environments Creation

    private void GenerateEnvironments()
    {
        if (environmentCreation)
        {
            GameObject Environments = new GameObject("Environments");
            Environments.transform.SetParent(transform);

            if (grassLike.Length > 0)
            {
                for (int i = 0; i < grndTop.Length; i++)
                {
                    int index = Random.Range(0, grassLike.Length);
                    Vector3 posGroundTop = new Vector3(grndTop[i].transform.position.x, grndTop[i].transform.position.y + 0.5f, grndTop[i].transform.position.z);
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
                for (int i = 0; i < grndTop.Length; i += Rand)
                {
                    int index = Random.Range(0, stoneOrBush.Length);
                    Vector3 postition = new Vector3(grndTop[i].transform.position.x + 0.6f, grndTop[i].transform.position.y + 0.5f, grndTop[i].transform.position.z);
                    Instantiate(stoneOrBush[index], postition, Quaternion.identity, Environments.transform);
                }
            }

            if (trees.Length > 0)
            {
                int Rand = Random.Range(minTreeDensity, maxTreeDensity + 1);
                for (int i = 0; i < grndTop.Length; i += Rand)
                {
                    int index = Random.Range(0, trees.Length);
                    Vector3 postition = new Vector3(grndTop[i].transform.position.x + 0.6f, grndTop[i].transform.position.y + 0.5f, grndTop[i].transform.position.z);
                    Instantiate(trees[index], postition, Quaternion.identity, Environments.transform);
                }
            }

            environmentCreation = false;
            enemiesCreation = true;
        }
    }

    #endregion

    #region Enemies Creation

    private void GenerateEnemies()
    {
        if (enemiesCreation)
        {
            GameObject Enemies = new GameObject("Enemies");
            Enemies.transform.SetParent(transform);

            for (int i = enemyDensity; i < grndTop.Length; i += 15)
            {
                int RandomEnemy = Random.Range(0, enemies.Length);
                Vector3 posGroundTop = new Vector3(grndTop[i].transform.position.x, grndTop[i].transform.position.y + 1.3f, grndTop[i].transform.position.z);
                Instantiate(enemies[RandomEnemy], posGroundTop, Quaternion.identity, Enemies.transform);
            }

            for (int i = enemyDensity / 2; i < halfTop.Length; i += 10)
            {
                int RandomEnemy = Random.Range(0, enemies.Length);
                Vector3 posHalfTop = new Vector3(halfTop[i].transform.position.x, halfTop[i].transform.position.y + 1.3f, halfTop[i].transform.position.z);
                Instantiate(enemies[RandomEnemy], posHalfTop, Quaternion.identity, Enemies.transform);
            }

            enemiesCreation = false;
        }
    }

    #endregion

    #endregion

    #region Others Method

    private void GenerateCheckpoints()
    {
        GameObject Checkpoints = new GameObject("Checkpoints");
        Checkpoints.transform.SetParent(transform);

        for (int i = 15; i < grndTop.Length; i += 20)
        {
            Vector3 posGroundTop = new Vector3(grndTop[i].transform.position.x, grndTop[i].transform.position.y + 0.5f, grndTop[i].transform.position.z);
            Instantiate(checkpoints, posGroundTop, Quaternion.identity, Checkpoints.transform);
        }
    }

    private void GeneratePickups()
    {
        GameObject Coins = new GameObject("Coins");
        Coins.transform.SetParent(transform);

        for (int i = 3; i < grndTop.Length; i++)
        {
            Vector3 posGroundTop = new Vector3(grndTop[i].transform.position.x, grndTop[i].transform.position.y + 1f, grndTop[i].transform.position.z);
            Instantiate(coins, posGroundTop, Quaternion.identity, Coins.transform);
        }

        for (int i = 0; i < halfTop.Length; i++)
        {
            Vector3 posHalfTop = new Vector3(halfTop[i].transform.position.x, halfTop[i].transform.position.y + 1f, halfTop[i].transform.position.z);
            Instantiate(coins, posHalfTop, Quaternion.identity, Coins.transform);
        }
    }

    private void FillLevels()
    {
        GameObject Filler = new GameObject("Filler");
        Filler.transform.SetParent(transform);

        for (int i = 0; i < grndTop.Length; i++)
        {
            Debug.DrawRay(new Vector2(grndTop[i].transform.position.x, grndTop[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(grndTop[i].transform.position.x, grndTop[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if (hit == false)
            {
                for (int j = 1; j <= 20; j++)
                {
                    Instantiate(filler, new Vector3(grndTop[i].transform.position.x, grndTop[i].transform.position.y - j,
                        grndTop[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }

        }

        for (int i = 0; i < grounds.Length; i++)
        {
            Debug.DrawRay(new Vector2(grounds[i].transform.position.x, grounds[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(grounds[i].transform.position.x, grounds[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if (hit == false)
            {
                for (int j = 1; j <= 19; j++)
                {
                    Instantiate(filler, new Vector3(grounds[i].transform.position.x, grounds[i].transform.position.y - j,
                        grounds[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }
        }

        for (int i = 0; i < hazards.Length; i++)
        {
            Debug.DrawRay(new Vector2(hazards[i].transform.position.x, hazards[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(hazards[i].transform.position.x, hazards[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if (hit == false)
            {
                for (int j = 1; j <= 20; j++)
                {
                    Instantiate(filler, new Vector3(hazards[i].transform.position.x, hazards[i].transform.position.y - j,
                        hazards[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }
        }
    }

    #endregion
}