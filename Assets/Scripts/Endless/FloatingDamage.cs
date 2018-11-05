using UnityEngine;
using TMPro;

public class FloatingDamage : MonoBehaviour {
    
    public float moveAmount;
    public float moveSpeed;

    private Vector3 moveDirection;
    private TMP_Text dmgText;
    private bool canMove;
    
    private void Start()
    {
        moveDirection = transform.up;
    }

    private void Update()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDirection, moveAmount * (moveSpeed * Time.deltaTime));
        }
    }

    public void SetText(string text)
    {
        dmgText = GetComponentInChildren<TMP_Text>();
        dmgText.text = text;
        dmgText.color = Color.red;
        canMove = true;
    }
}
