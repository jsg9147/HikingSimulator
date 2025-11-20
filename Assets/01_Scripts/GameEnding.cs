using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public GameObject beamEffect;

    private void Start()
    {
        beamEffect.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed += InteractionPlayer;
            InteractionUIManager.instance.AddInteractionObject(transform, "E", "Pray");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed -= InteractionPlayer;
            InteractionUIManager.instance.DisableUIForObject(transform);
        }
    }

    void InteractionPlayer()
    {
        GameManager.instance.playerStateController.Ending();
        GameManager.instance.playerTransfrom.position = transform.position + (Vector3.up * 1.7f);
        GameManager.instance.playerTransfrom.eulerAngles = new(0, 37.5f, 0);
        CameraController.instance.ToggleDayNight(false);
        CameraController.instance.SetEndingPos();
        //CameraController.instance.EndingCameraEffect();
        beamEffect.gameObject.SetActive(true);
        Invoke("LoadMainScene", 13f);

    }

    void LoadMainScene()
    {
        GetComponent<SteamAchievement>().GameEnding();
        CameraController.instance.CameraChange();
        UIManager.instance.CursorChange(CursorLockMode.None);
        SceneManager.LoadScene(0);
        SingletonManager.instance.ResetAllSingletons();
        SceneManager.LoadScene("Main");  // 'Main' æ¿¿ª ∑ŒµÂ
    }
}
