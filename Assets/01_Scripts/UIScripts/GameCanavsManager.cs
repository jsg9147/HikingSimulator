using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanavsManager : MonoBehaviour
{
    public static GameCanavsManager instance;
    private Canvas canvas;

    private void Awake()
    {
        if(instance == null)
            instance = this;

        canvas = GetComponent<Canvas>();
    }
    void Start()
    {
        if (Camera.main != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
