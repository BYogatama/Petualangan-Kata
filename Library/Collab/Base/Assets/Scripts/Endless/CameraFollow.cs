using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Camera Properties")]
    public Renderer target;
    public Rect deadzone;
    public Vector3 smoothPos;
    public Rect[] limits;

    [Header("Camera Lock")]
    public bool lockCamera;
    public Vector3 minCameraPos;
    public Vector3 maxCameraPos;
    
    [Header("Level Edge")]
    public EdgeCollider2D leftEdge;
    public EdgeCollider2D bottomEdge;
    public EdgeCollider2D rightEdge;

    protected Camera _camera;
    protected Vector3 _currentVelocity;

    private EndlessManager manager;

    Vector3 exitPos;

    private IEnumerator FindExit()
    {
        LevelGeneration levelGen = FindObjectOfType<LevelGeneration>();
        if (!levelGen.LevelCreationFinished)
        {
            yield return null;
        }

        exitPos = GameObject.FindGameObjectWithTag("ExitLevel").transform.position;
    }

    public void Start()
    {
        StartCoroutine(FindExit());

        smoothPos = target.transform.position;
        smoothPos.z = transform.position.z;
        _currentVelocity = Vector3.zero;

        _camera = GetComponent<Camera>();
        if (!_camera.orthographic)
        {
            Debug.LogError("Camera Follow script require an orthographic camera!");
            Destroy(this);
        }

        SetCameraLimits();
        SetLevelEdge();

    }

    public void Update()
    {
        if (target == null)
        {
            return;
        }

        float localX = target.transform.position.x - transform.position.x;
        float localY = target.transform.position.y - transform.position.y;
        
        if (localX < deadzone.xMin)
        {
            smoothPos.x += localX - deadzone.xMin;
        }
        else if (localX > deadzone.xMax)
        {
            smoothPos.x += localX - deadzone.xMax;
        }

        if (localY < deadzone.yMin)
        {
            smoothPos.y += localY - deadzone.yMin;
        }
        else if (localY > deadzone.yMax)
        {
            smoothPos.y += localY - deadzone.yMax;
        }

        Rect camWorldRect = new Rect();
        camWorldRect.min = new Vector2(smoothPos.x - _camera.aspect * _camera.orthographicSize, smoothPos.y - _camera.orthographicSize);
        camWorldRect.max = new Vector2(smoothPos.x + _camera.aspect * _camera.orthographicSize, smoothPos.y + _camera.orthographicSize);

        for (int i = 0; i < limits.Length; ++i)
        {
            if (limits[i].Contains(target.transform.position))
            {
                Vector3 localOffsetMin = limits[i].min + camWorldRect.size * 0.5f;
                Vector3 localOffsetMax = limits[i].max - camWorldRect.size * 0.5f;

                localOffsetMin.z = localOffsetMax.z = smoothPos.z;

                smoothPos = Vector3.Max(smoothPos, localOffsetMin);
                smoothPos = Vector3.Min(smoothPos, localOffsetMax);

                break;
            }
        }

        Vector3 current = transform.position;
        current.x = smoothPos.x; // we don't smooth horizontal movement

        transform.position = Vector3.SmoothDamp(current, smoothPos, ref _currentVelocity, 0.1f);

        if (lockCamera)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x),
                Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y),
                Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
        }

    }

    private void SetCameraLimits()
    {
        for (int i = 0; i < limits.Length; i++)
        {
            limits[i].width = exitPos.x + 10f;

            if (exitPos.y == 0)
            {
                limits[i].y = (exitPos.y - 9f);
            }
            else if(exitPos.y < 0)
            {
                limits[i].y = (exitPos.y - 9f);
            }
            else if (exitPos.y == 1)
            {
                limits[i].y = (exitPos.y - 9f);
            }
            else if (exitPos.y > 1)
            {
                limits[i].y = (exitPos.y - 9f);
                if (limits[i].y > 0)
                {
                    limits[i].y *= -1;
                }
                else if (limits[i].y == 0)
                {
                    limits[i].y = -9f;
                }
                
            }
        }

        maxCameraPos = new Vector3(exitPos.x + 0.6f, minCameraPos.y + 20, transform.position.z);
        minCameraPos = new Vector3(minCameraPos.x, limits[0].y, minCameraPos.z);
    }

    private void SetLevelEdge()
    {
        Vector2[] colliderPoints;

        leftEdge.transform.position = new Vector3(-0.5f, leftEdge.transform.position.y, leftEdge.transform.position.z);
        bottomEdge.transform.position = new Vector3(bottomEdge.transform.position.x, limits[0].y - 1, bottomEdge.transform.position.z);
        rightEdge.transform.position = new Vector3(exitPos.x + 9.5f, rightEdge.transform.position.y, rightEdge.transform.position.z);

        colliderPoints = bottomEdge.points;
        colliderPoints[0] = new Vector2(exitPos.x + 10f, 0);
        bottomEdge.points = colliderPoints;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(CameraFollow))]
public class DeadZonEditor : Editor
{
    public void OnSceneGUI()
    {
        CameraFollow cam = target as CameraFollow;

        Vector3[] vert =
        {
            cam.transform.position + new Vector3(cam.deadzone.xMin, cam.deadzone.yMin, 0),
            cam.transform.position + new Vector3(cam.deadzone.xMax, cam.deadzone.yMin, 0),
            cam.transform.position + new Vector3(cam.deadzone.xMax, cam.deadzone.yMax, 0),
            cam.transform.position + new Vector3(cam.deadzone.xMin, cam.deadzone.yMax, 0)
        };

        Color transp = new Color(0, 0, 0, 0);
        Handles.DrawSolidRectangleWithOutline(vert, transp, Color.red);

        for (int i = 0; i < cam.limits.Length; ++i)
        {
            Vector3[] vertLimit =
           {
                new Vector3(cam.limits[i].xMin, cam.limits[i].yMin, 0),
                new Vector3(cam.limits[i].xMax, cam.limits[i].yMin, 0),
                new Vector3(cam.limits[i].xMax, cam.limits[i].yMax, 0),
                new Vector3(cam.limits[i].xMin, cam.limits[i].yMax, 0)
            };

            Handles.DrawSolidRectangleWithOutline(vertLimit, transp, Color.green);
        }
    }
}
#endif