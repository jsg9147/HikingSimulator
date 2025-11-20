using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class BGMPlayer : MonoBehaviour
{
    public string bgmName;
    void Start()
    {
        MasterAudio.StartPlaylist(bgmName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
