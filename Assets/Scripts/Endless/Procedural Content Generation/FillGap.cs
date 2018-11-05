using UnityEngine;

public class FillGap : MonoBehaviour
{
    [SerializeField]
    GameObject filler;

    GameObject Filler;

    public void FillLevel()
    {
        Filler = new GameObject("Filler");
        Filler.transform.SetParent(transform);

        GameObject[] groundTop = GameObject.FindGameObjectsWithTag("GroundTop");
        GameObject[] ground    = GameObject.FindGameObjectsWithTag("Ground");
        GameObject[] hazard    = GameObject.FindGameObjectsWithTag("Hazard");

        for(int i = 0; i < groundTop.Length; i++)
        {
            Debug.DrawRay(new Vector2(groundTop[i].transform.position.x, groundTop[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(groundTop[i].transform.position.x, groundTop[i].transform.position.y - 1f),
                new Vector2(0, -0.5f));

            if(hit == false)
            {
                for (int j = 1; j <= 10; j++)
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
                for (int j = 1; j <= 9; j++)
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
                for (int j = 1; j <= 10; j++)
                {
                    Instantiate(filler, new Vector3(hazard[i].transform.position.x, hazard[i].transform.position.y - j,
                        hazard[i].transform.position.z), Quaternion.identity, Filler.transform);
                }
            }
        }

    }

}
