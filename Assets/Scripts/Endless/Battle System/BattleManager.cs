
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour {

    #region Inspector

    [Header("Timer")]
    public float startTime;
    public float wordTimer;

    [Header("Word Manager")]
    public WordController activeWord;
    public bool hasActiveWord;

    [Header("Word Spawner")]
    public GameObject wordObject;
    public Transform canvas;

    [Header("Battle System")]
    public TMP_Text status;
    public TMP_Text timerText;
    public GameObject floatingDamage;

    [Header("Position")]
    public Transform playerPos;
    public Transform enemyPos;

    #endregion

    #region Privates

    string lastActiveWord;

    int playerHealth;
    int playerDamage;
    int enemyHealth;
    int enemyDamage;

    int timer;

    bool playerTurn;
    bool playerLose;
    bool enemyLose;
    bool wordCreated;

    string countValue;

    #endregion

    #region Publics
    

    #endregion

    #region Object References

    Animator playerAnim;
    Animator enemyAnim;

    EndlessManager GameSystem;

    GameObject player;
    GameObject enemy;

    Transform battleArea;

    Slider playerHealthBar;
    Slider enemyHealthBar;

    WordDisplay wordDisplay;

    #endregion

    #region UnityMessages

    public IEnumerator StartBattle()
    {
        enemyLose = false;
        playerLose = false;
        playerTurn = true;

        wordTimer = startTime;

        AudioController.Instance.MuteMusic("Jump!", true);
        AudioController.Instance.MuteMusic("Crack The Code", false);

        //Find Game Manager
        GameSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EndlessManager>();

        //Set Local Player & Enemy Health Value to Game Manager Health Val
        playerHealth = GameSystem.curPlayerHealth;
        enemyHealth = GameSystem.curEnemyHealth;

        //Spawn Player & Enemey in Battle Area
        player = Instantiate(GameSystem.refPlayer, playerPos.position, Quaternion.identity, transform);
        enemy = Instantiate(GameSystem.refEnemy, enemyPos.position, Quaternion.identity, transform);

        player.transform.localScale = new Vector3(1, 1, 1);
        enemy.transform.localScale = new Vector3(1, 1, 1);

        playerAnim = player.GetComponent<Animator>();
        enemyAnim = enemy.GetComponent<Animator>();

        playerAnim.SetLayerWeight(1, 1);
        enemyAnim.SetLayerWeight(1, 1);

        //Set Player & Enemy Health Text to Player & Enemy Health Value
        GameSystem.playerHealthText.text = playerHealth.ToString();
        GameSystem.enemyHealthText.text = enemyHealth.ToString();

        //Set Player & Enemy Health Bar Value According to Player & Enemy Health Value
        GameSystem.playerHealthBar.value = playerHealth;
        GameSystem.enemyHealthBar.value = enemyHealth;
        GameSystem.enemyHealthBar.maxValue = GameSystem.maxEnemyHealth;

        status.text = "Pertarungan Dimulai";
        status.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        status.gameObject.SetActive(false);
        
        //Wait for 2 Sec & Start Player Turn
        yield return new WaitForSeconds(.5f);
        StartCoroutine(StartPlayerTurn());
    }

    private void Update()
    {
        if (GameSystem.isInBattle)
        {
            //Check every frame for Player Turn
            if (playerTurn)
            {
                timerText.text = timer.ToString();

                if (wordCreated)
                {
                    foreach (char letter in Input.inputString.ToLower())
                    {
                        TypeLetter(letter);
                    }

                    wordTimer -= Time.deltaTime;
                }

                if(wordTimer <= 0)
                {
                    wordCreated = false;
                    hasActiveWord = false;
                    wordDisplay.RemoveWord();
                    StartCoroutine(SwitchTurn());
                    wordTimer = startTime;
                }

                timer = System.Convert.ToInt32(wordTimer);
            }
            else
            {
                timerText.gameObject.SetActive(false);
            }

        }
    }

    #endregion

    #region Word Commands

    public void CreateWord()
    {
        WordController Words = new WordController(WordGenerator.GetRandomWord(), SpawnWord());
        activeWord = Words;
        startTime = Words.word.Length;
        wordTimer = startTime + 1;
        lastActiveWord = Words.word;
    }

    public void ResetWord()
    {
        WordController Words = new WordController(lastActiveWord, SpawnWord());
        activeWord = Words;
    }

    public void TypeLetter(char letter)
    {
        if (hasActiveWord)
        {
            if (activeWord.GetNextLetter() == letter)
            {
                AudioController.Instance.PlayFX("Type");
                activeWord.TypeLetter();
            }
            else if(activeWord.GetNextLetter() != letter)
            {
                ResetWord();
                hasActiveWord = false;
            }
        }
        else
        {
            if (activeWord.GetNextLetter() == letter)
            {
                AudioController.Instance.PlayFX("Type");
                hasActiveWord = true;
                activeWord.TypeLetter();
            }
        }

        if (hasActiveWord && activeWord.WordTyped())
        {
            hasActiveWord = false;

            //Handle Battle
            if (!playerLose)
            {
                PlayerAttack();
                StartCoroutine(SwitchTurn());

                wordCreated = false;
                wordTimer = startTime;
            }
        }
    }

    #endregion

    #region Word Spawner

    public WordDisplay SpawnWord()
    {
        wordDisplay = wordObject.GetComponent<WordDisplay>();

        return wordDisplay;
    }

    #endregion

    #region Battle Commands

    private IEnumerator SwitchTurn()
    {
        //Check if Player Win or Lose
        StartCoroutine(BattleStatus());

        //Set Player & Enemy Health Text to Player & Enemy Health Value
        GameSystem.playerHealthText.text = playerHealth.ToString();
        GameSystem.enemyHealthText.text = enemyHealth.ToString();

        //Set Player & Enemy Health Bar Value According to Player & Enemy Health Value
        GameSystem.playerHealthBar.value = playerHealth;
        GameSystem.enemyHealthBar.value = enemyHealth;
        
        playerTurn = !playerTurn;

        if (GameSystem.isInBattle) {
            //If it's Player Turn & player not LOSE
            //Then Start Player Turn
            if (playerTurn && !playerLose)
            {
                yield return new WaitForSeconds(.5f);
                StartCoroutine(StartPlayerTurn());
            }
            //If not Player Turn & Enemy not LOSE
            //Then Enemy Turn
            else if (!playerTurn && !enemyLose)
            {
                yield return new WaitForSeconds(.5f);
                StartCoroutine(StartEnemyTurn());
            }
        }
    }

    private IEnumerator StartPlayerTurn()
    {
        status.text = "Giliran Kamu Menyerang, Bersiap !";
        status.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        status.gameObject.SetActive(false);
        CreateWord();
                
        timerText.gameObject.SetActive(true);
        wordCreated = true;

        int animIdx = Random.Range(1, 3);
        playerAnim.SetInteger("Attack", animIdx);
    }

    private IEnumerator StartEnemyTurn()
    {
        status.text = "Giliran Musuh Menyerang";
        status.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2f);
        status.gameObject.SetActive(false);

        yield return new WaitForSeconds(Random.Range(1, 2));
        EnemyAttack();
        StartCoroutine(SwitchTurn());
        
    }

    private IEnumerator BattleStatus()
    {
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            GameSystem.playerHealthText.text = playerHealth.ToString();
            GameSystem.playerHealthBar.value = playerHealth;

            playerLose = true;
            playerAnim.SetBool("IsDead", playerLose);
            yield return new WaitForSeconds(0.5f);
            Destroy(player);
            status.text = "Kamu Kalah !";
            status.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(2);
            FinishBattle();
        }

        if (enemyHealth <= 0)
        {
            enemyHealth = 0;
            GameSystem.enemyHealthText.text = enemyHealth.ToString();
            GameSystem.enemyHealthBar.value = enemyHealth;

            enemyLose = true;
            enemyAnim.SetBool("IsDead", enemyLose);
            yield return new WaitForSeconds(0.5f);
            Destroy(enemy);
            status.text = "Kamu Menang !";
            status.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(2);
            FinishBattle();
        }
    }

    private void PlayerAttack()
    {
        StartCoroutine(Attack(player, enemy));
        
        playerDamage = Random.Range(GameSystem.playerMinDamage, GameSystem.playerMaxDamage + 1);
        FloatDamage(playerDamage.ToString(), enemy);
        enemyHealth -= playerDamage;
    }

    private void EnemyAttack()
    {
        StartCoroutine(Attack(enemy, player));

        enemyDamage = Random.Range(GameSystem.enemyMinDamage, GameSystem.enemyMaxDamage + 1);
        FloatDamage(enemyDamage.ToString(), player);
        playerHealth -= enemyDamage;
    }

    #endregion

    #region Privates Methods
    
    private IEnumerator Attack(GameObject Attacker, GameObject Target)
    {
        Animator AttackerAnim = Attacker.GetComponent<Animator>();
        Animator TargetAnim = Target.GetComponent<Animator>();

        Vector2 AttackerPos = Attacker.transform.position;
        Vector2 TargetPos = Target.transform.position;

        Vector2 lastAttackerPos = AttackerPos;

        Attacker.transform.position = Vector2.MoveTowards(AttackerPos, TargetPos, 4.5f);
        AudioController.Instance.PlayFX("Slash");
        AttackerAnim.SetBool("IsAttack", true);

        yield return new WaitForSeconds(.2f);
        TargetAnim.SetTrigger("Attacked");

        yield return new WaitForSeconds(.6f);
        AttackerAnim.SetBool("IsAttack", false);
        Attacker.transform.position = Vector2.MoveTowards(AttackerPos, lastAttackerPos, 0f);
    }

    private void FloatDamage(string dmgText, GameObject target)
    {
        float killTime = 0.5f;

        Vector3 position = new Vector3(target.transform.position.x, target.transform.position.y + 1.5f, target.transform.position.z);
        GameObject floatDmg = Instantiate(floatingDamage, position, Quaternion.identity);

        floatDmg.SetActive(true);
        floatDmg.GetComponent<FloatingDamage>().SetText("- " + dmgText);

        Destroy(floatDmg, killTime);
    }

    private void FinishBattle()
    {
        status.gameObject.SetActive(false);
        GameSystem.curPlayerHealth = playerHealth;

        if (playerLose)
        {
            GameSystem.Respawn(GameSystem.playerRespawnPos);
            GameSystem.refEnemy.GetComponent<EnemiesBrains>().curHealth = enemyHealth;
            
            Destroy(player);
            Destroy(enemy);            
        }
        else if (enemyLose)
        {
            Destroy(player);
            Destroy(GameSystem.refEnemy.gameObject);

            GameSystem.curPlayerHealth += 20;
            GameSystem.score += 500;
        }

        GameSystem.fadeLevel.FadeIn();
        GameSystem.battleScene.SetActive(false);
        GameSystem.mainScene.SetActive(true);

        GameSystem.battleUI.SetActive(false);
        GameSystem.mainUI.SetActive(true);

        GameSystem.isInBattle = false;

        AudioController.Instance.MuteMusic("Jump!", false);
        AudioController.Instance.MuteMusic("Crack The Code", true);
    }

    #endregion
}
