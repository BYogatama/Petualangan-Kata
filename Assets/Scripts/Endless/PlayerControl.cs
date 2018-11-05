using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : PhysicEngine
{
    #region Inspector

    [Header("Configs")]
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    #endregion

    #region Privates
    
    private Vector2 move = Vector2.zero;

    #endregion

    #region References

    private Animator playerAnimator;
    private EndlessManager manager;
    BattleManager battle;

    #endregion

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EndlessManager>();
        battle = manager.battleScene.GetComponent<BattleManager>();
    }

    protected override void ComputeVelocity()
    {

        playerAnimator.SetBool("Grounded", grounded);
        playerAnimator.SetFloat("VelocityX", Mathf.Abs(velocity.x) / maxSpeed);

        if (!manager.isInBattle)
        {
            move.x = Input.GetAxis("Horizontal");
            
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }

            targetVelocity = move * maxSpeed;

            if(move.x < 0)
            {
                if (transform.localScale.x == 1)
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }
            }
            else if (move.x > 0)
            {
                if(transform.localScale.x != 1)
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }
            }
        }
    }

    #region Private Methods

    public void Jump()
    {
        if (grounded)
        {
            AudioController.Instance.PlayFX("Jump");
            velocity.y = jumpTakeOffSpeed;
        }
    }

    #endregion

    #region Collision

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Hazard"))
        {
            manager.curPlayerHealth -= 10;
            velocity.y = jumpTakeOffSpeed;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemies"))
        {
            if (!manager.isInBattle)
            {
                manager.battleScene.SetActive(true);
                manager.mainScene.SetActive(false);

                manager.battleUI.SetActive(true);
                manager.mainUI.SetActive(false);

                manager.isInBattle = true;

                manager.refPlayer = gameObject;
                manager.refEnemy = other.gameObject;

                manager.maxEnemyHealth = other.gameObject.GetComponent<EnemiesBrains>().maxHealth;
                manager.curEnemyHealth = other.gameObject.GetComponent<EnemiesBrains>().curHealth;
                manager.enemyMinDamage = other.gameObject.GetComponent<EnemiesBrains>().minDamage;
                manager.enemyMaxDamage = other.gameObject.GetComponent<EnemiesBrains>().maxDamage;

                manager.fadeLevel.FadeIn();
                battle.StartCoroutine(battle.StartBattle());
            }
        }

        if (other.tag == "Checkpoint")
        {
            manager.playerRespawnPos = other.transform.position;
        }

        if(other.tag == "LevelEdge")
        {
            manager.curPlayerHealth -= manager.curPlayerHealth;
        }

        if(other.tag == "Coins")
        {
            AudioController.Instance.PlayFX("Coin");
            manager.score += 10;
            Destroy(other.gameObject);
        }

        if(other.tag == "ExitPoint")
        {
            manager.LoadScene("EndlessSelesai");
        }
    }

    #endregion
}
