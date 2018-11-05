using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {

    private Animator anim;


    public GameObject leftDetect;
    public GameObject rightDetect;
    public bool checkpointReached;

    EndlessManager manager;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        manager = FindObjectOfType<EndlessManager>();
    }

    private void Update()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(leftDetect.transform.position, new Vector2(0, -1), 0.1f);
        RaycastHit2D hitRight = Physics2D.Raycast(rightDetect.transform.position, new Vector2(0, -1), 0.1f);

        Debug.DrawRay(leftDetect.transform.position, Vector2.down * 0.1f);
        Debug.DrawRay(rightDetect.transform.position, Vector2.down * 0.1f);

        if (hitLeft == false)
        {
            transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
        }

        if (hitRight == false)
        {
            transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            checkpointReached = true;
            anim.SetBool("Reached", checkpointReached);
            manager.playerRespawnPos = transform.position;
        }
    }
}
