using UnityEngine;
using System;
using DarkTonic.MasterAudio;

public class InputManager : SingletonBase<InputManager>
{
    // 이벤트 정의: 입력이 발생했을 때 다른 시스템에서 구독할 수 있음
    public event Action OnInteractKeyPressed;
    public event Action OnCancelKeyPressed;

    public event Action OnInventoryKeyPressed;

    public event Action OnPickUpKeyPressed;
    public event Action OnLayingKeyPressed;
    public event Action OnSmartPhonePressed;
    public event Action OnQuestPopupPressed;

    public event Action OnEscKeyPressed;
    // 필요한 추가 이벤트들

    void Update()
    {
        // 상호작용 키
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteractKeyPressed?.Invoke();
            MasterAudio.PlaySound3DAtTransform("Interaction", GameManager.instance.playerMovement.transform);
        }

        // 취소 키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCancelKeyPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            OnInventoryKeyPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            OnPickUpKeyPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnLayingKeyPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            OnSmartPhonePressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            OnQuestPopupPressed?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscKeyPressed?.Invoke();
        }
    }
}
