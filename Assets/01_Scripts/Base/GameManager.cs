using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

public class GameManager : SingletonBase<GameManager>
{
    public Transform playerTransfrom;
    public PlayerMovement playerMovement;
    public PlayerStateController playerStateController;
    public TentPlacement tentPlacement;
    public SurvivalStatsManager survivalStatsManager;

    public int distance;

    bool isGameOver = false;

    private void Start()
    {
        //SetResolution(1600, 900, false);
        MasterAudio.StartPlaylist("Road");
    }

    public void SetResolution(int width, int height, bool fullscreen)
    {
        Screen.SetResolution(width, height, fullscreen);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (QuestManager.instance != null)
        {
            QuestManager.instance.CompanionPositionUpdate();
        }
    }
    public void PlayerRecovery()
    {
        UIManager.instance.FadeOutAndIn();
        WeatherManager.instance.SkipTime();
        
        survivalStatsManager.Recovery();
    }

    public bool IsGameOver => isGameOver;
    public void GameOver()
    {
        if (isGameOver)
            return;

        MasterAudio.PlaySound3DAtTransform("Death", playerTransfrom);
        MasterAudio.ChangePlaylistByName("Death");
        CameraController.instance.CameraChange();
        UIManager.instance.CursorChange(CursorLockMode.None);
        
        playerStateController.SetState(PlayerState.Death);
        UIManager.instance.FadeOutOnly();
        isGameOver = true;
    }
}
