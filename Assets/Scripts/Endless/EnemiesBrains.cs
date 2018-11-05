using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesBrains : PhysicEngine {

    [Header("Configs")]
    public float speed;
    public Transform topOriginPoint;
    public Transform bottomOriginPoint;
    public Vector2 moveDirection;
    public Vector2 raycastDir;
    public float raycastRange;

    [Header("Stats")]
    public int maxHealth;
    public int curHealth;
    public int minDamage;
    public int maxDamage;
    
    private EndlessManager manager;

    private Animator enemyAnimator;

    private void Awake()
    {
        manager = FindObjectOfType<EndlessManager>();
        enemyAnimator = gameObject.GetComponent<Animator>();

        curHealth = maxHealth;
    }

    protected override void ComputeVelocity()
    {
        if (!manager.isInBattle)
        {
            enemyAnimator.SetFloat("VelocityX", Mathf.Abs(velocity.x) / speed);
            targetVelocity = moveDirection * speed;

            RaycastHit2D topHit = Physics2D.Raycast(topOriginPoint.position, raycastDir, raycastRange);
            RaycastHit2D botHit = Physics2D.Raycast(bottomOriginPoint.position, raycastDir, raycastRange);

            Debug.DrawRay(topOriginPoint.position, raycastDir * raycastRange);
            Debug.DrawRay(bottomOriginPoint.position, raycastDir * raycastRange);

            if (topHit == true)
            {
                if (topHit.collider.CompareTag("GroundTop") || topHit.collider.CompareTag("Ground")
                    || topHit.collider.CompareTag("Enemies"))
                {
                    Flip();
                    moveDirection *= -1;
                    raycastDir *= -1;
                }
            }

            if (botHit == false)
            {
                Flip();
                moveDirection *= -1;
                raycastDir *= -1;
            }
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
