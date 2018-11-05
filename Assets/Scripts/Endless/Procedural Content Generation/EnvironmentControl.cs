using UnityEngine;

public class EnvironmentControl : MonoBehaviour {

    [SerializeField]
    private Transform objectDetectLeft;
    [SerializeField]
    private Transform objectDetectRight;

    private void Start()
    {
        RaycastHit2D objectLeft = Physics2D.Raycast(objectDetectLeft.position, new Vector2(0, -1), 0.1f);
        RaycastHit2D objectRight = Physics2D.Raycast(objectDetectRight.position, new Vector2(0, -1), 0.1f);

        Debug.DrawLine(objectDetectLeft.position, Vector2.down * 0.1f);
        Debug.DrawLine(objectDetectRight.position, Vector2.down * 0.1f);

        if (objectLeft == false)
        {
            gameObject.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }

        if (objectRight == false)
        {
            gameObject.transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }
    }

}
