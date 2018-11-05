using UnityEngine;
using UnityEngine.SceneManagement;

public class AlignLevel : MonoBehaviour {

    public int patternSize;
    public Transform startPoints;

    Vector3 curPosition;
    RaycastHit2D alignPoints;

    bool Aligned;
    bool LookUp;

    float positionX;
    float positionY;
    float positionZ;

    public void Align()
    {
        LookUp  = true;
        Aligned = false;

        int i = 0;

        while (!Aligned)
        {
            //Create Raycast for finding ExitPoints
            alignPoints = Physics2D.Raycast(startPoints.position, new Vector2(-1, 0), 2f);

            //Get X,Y,Z position of GameObject
            positionX = gameObject.transform.position.x;
            positionY = gameObject.transform.position.y;
            positionZ = gameObject.transform.position.z;
            
            if(alignPoints == false && LookUp)
            {
                //Check if Position Y Negative or Positive
                //If Negative Do This
                if (positionY < 0)
                {
                    gameObject.transform.position = new Vector3(positionX, (positionY * -1) + 1, positionZ);
                }
                //If Postitive Do This
                else if (positionY >= 0)
                {
                    gameObject.transform.position = new Vector3(positionX, positionY + 1, positionZ);
                }
                
                //If Position Y == 10 Do This
                if (positionY == 20)
                {
                    LookUp = false;
                }

            }

            else if (alignPoints == false && !LookUp)
            {
                gameObject.transform.position = new Vector3(positionX, positionY - 1, positionZ);
                if(positionY == -20)
                {
                    LookUp = true;
                }
            }

            else if (alignPoints == true && !alignPoints.collider.CompareTag("ExitPoints"))
            {
                gameObject.transform.position = new Vector3(positionX, positionY + 1, positionZ);
            }

            else if (alignPoints == true && alignPoints.collider.CompareTag("ExitPoints"))
            {
                curPosition = gameObject.transform.position;
                //Set Position of Level Pattern
                gameObject.transform.position = curPosition;

                //Destroy startPoints & exitPoints when aligned
                Destroy(startPoints.gameObject);
                Destroy(alignPoints.collider.gameObject);
                    
                //Set aligned to true
                Aligned = true;
            }
            

            i++;
            if (i == 5000)
            {
                Debug.Log("Too Long to Align, Object : " + gameObject.name + " Position : " + transform.position);
                RestartScene();
                break;
            }
        }
    }


    void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
