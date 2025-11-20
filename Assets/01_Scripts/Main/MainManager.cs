using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;
using Steamworks;

public class MainManager : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        MasterAudio.StartPlaylist("Main");
        GameObject obj = GameObject.Find("SurveCamera");
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
