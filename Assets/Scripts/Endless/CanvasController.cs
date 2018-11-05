using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour {

    public EndlessManager manager;
    public Camera mainCamera;
    public Camera battleCamera;

    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (!manager.isInBattle)
        {
            canvas.worldCamera = mainCamera;
        }
        else
        {
            canvas.worldCamera = battleCamera;
        }
    }


}
