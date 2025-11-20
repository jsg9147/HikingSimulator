using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausePopup : MonoBehaviour
{
    public GameObject popup;

    public Button continueBtn;
    public Button optionButton;
    public Button exitBtn;
    void Start()
    {
        popup.SetActive(false);

        exitBtn.onClick.AddListener(MainScene);
        continueBtn.onClick.AddListener(Continue);
        optionButton.onClick.AddListener(PopupOff);
    }

    public void PausePopupOn()
    {
        if (popup.activeSelf)
        {
            Continue();
        }
        else
        {
            Time.timeScale = 0f;
            popup.SetActive(true);
            UIManager.instance.CursorChange(CursorLockMode.None);
        }
    }

    void MainScene()
    {
        CameraController.instance.CameraChange();
        SingletonManager.instance.ResetAllSingletons();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main");
    }

    void Continue()
    {
        Time.timeScale = 1.0f;
        UIManager.instance.CursorChange(CursorLockMode.Locked);
        popup.SetActive(false);
    }

    void PopupOff()
    {
        popup.SetActive(false);
    }
}
