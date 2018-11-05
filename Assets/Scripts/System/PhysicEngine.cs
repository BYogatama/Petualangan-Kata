using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicEngine : MonoBehaviour {

    public float minGroundNormalY = 0.65f;
    public float gravityModifer = 1f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contacFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    private void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        contacFilter.useTriggers = false;
        contacFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contacFilter.useLayerMask = true;
    }

    private void Update()
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }

    private void FixedUpdate()
    {
        velocity += gravityModifer * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;
        
        Vector2 deltaPos = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPos.x;

        Movement(move, false);

        move = Vector2.up * deltaPos.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if(distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contacFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            for (int i = 0; i < count; i++)
            {
                PlatformEffector2D platform = hitBuffer[i].collider.GetComponent<PlatformEffector2D>();
                if (!platform || (hitBuffer[i].normal == Vector2.up && velocity.y < 0 && yMovement))
                {
                    hitBufferList.Add(hitBuffer[i]);
                }
            }

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modofiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modofiedDistance < distance ? modofiedDistance : distance;
            }
        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }
}
