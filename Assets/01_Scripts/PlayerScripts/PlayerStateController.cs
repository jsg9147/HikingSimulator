using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    private PlayerAnimator playerAnimator;
    private PlayerMovement playerMovement;
    private SurvivalStatsManager survivalStatsManager; // SurvivalStatsManager 컴포넌트 참조

    private PlayerState currentState = PlayerState.Idle;
    private Transform tentCenter;

    public Transform cameraTargetPosition; // 카메라의 목표 위치
    public Vector3 cameraTargetRotation; // 카메라의 목표 회전 (Euler angles)
    public bool isCampingPlace;

    private Quaternion originalPlayerRotation; // 플레이어의 원래 회전
    private Vector3 originalCameraPosition; // 카메라의 원래 위치
    private Quaternion originalCameraRotation; // 카메라의 원래 회전

    private Transform tentTrans;
    private float tentInteractionCooldown = 3f;
    private bool canInteractWithTent = true;
    private bool isStunned = false; // Stun 상태 여부를 추적하는 변수

    public PlayerState CurrentState => currentState;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        playerMovement = GetComponent<PlayerMovement>();
        survivalStatsManager = GetComponent<SurvivalStatsManager>(); // SurvivalStatsManager 초기화

        originalPlayerRotation = transform.rotation;
        originalCameraPosition = Camera.main.transform.localPosition;
        originalCameraRotation = Camera.main.transform.rotation;

        GameManager.instance.playerStateController = this;
    }

    void Update()
    {
        // Stun 상태일 때는 아무 행동도 하지 않음
        if (!isStunned)
        {
            HandleState();
        }
    }

    public void SetTentTransform(Transform tent) => tentCenter = tent;

    /// <summary>
    /// Moves the player to the center of the tent and adjusts the camera position.
    /// </summary>
    void MoveToTentCenter()
    {
        if (tentCenter == null || cameraTargetPosition == null)
            return;

        try
        {
            transform.position = tentCenter.position;
            transform.rotation = Quaternion.Euler(0, tentCenter.eulerAngles.y, 0);

            Camera.main.transform.localPosition = cameraTargetPosition.localPosition;
            Camera.main.transform.rotation = Quaternion.Euler(cameraTargetRotation);

            SetState(PlayerState.Laying);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.LogError($"MoveToTentCenter Error: {ex}");
        }
    }

    /// <summary>
    /// Returns the player to their original position and adjusts the camera position.
    /// </summary>
    void ReturnToOriginalPosition()
    {
        transform.rotation = originalPlayerRotation;

        Camera.main.transform.localPosition = originalCameraPosition;
        Camera.main.transform.rotation = originalCameraRotation;

        playerAnimator.PlayLayingAnimation(false);
        currentState = PlayerState.Motion;
    }

    /// <summary>
    /// Handles the player's state and updates animations and movement based on the current state.
    /// </summary>
    void HandleState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                playerAnimator.PlayWalkingAnimation(false);
                playerMovement.enabled = true; // Idle 상태에서는 이동 가능
                break;
            case PlayerState.Walking:
                playerAnimator.PlayWalkingAnimation(true);
                break;
            case PlayerState.Sitting:
                playerAnimator.PlaySitAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Laying:
                playerAnimator.PlayLayingAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Working:
                playerAnimator.PlayWorkingAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Stun:
                playerAnimator.PlayerStunAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
        }
    }

    /// <summary>
    /// Sets the player's state and updates the recovery speed if the player is laying in the tent.
    /// </summary>
    /// <param name="newState">The new state to set.</param>
    public void SetState(PlayerState newState)
    {
        if (currentState == PlayerState.Death)
            return;

        if (newState == PlayerState.Death)
        {
            currentState = PlayerState.Death;
            playerAnimator.SetDie();
            isStunned = true;
            PlayerAnimationPlay(); // 애니메이션 재생
            return;
        }

        // Stun 상태일 때는 다른 상태로 전환되지 않도록 막음
        if (isStunned && newState != PlayerState.Stun)
            return;

        // Walking 상태 해제 로직 통합
        if (newState != PlayerState.Walking && currentState == PlayerState.Walking)
        {
            playerAnimator.PlayWalkingAnimation(false);
        }

        currentState = newState;

        // 애니메이션 재생을 PlayerAnimationPlay로 위임
        PlayerAnimationPlay();

        if (currentState == PlayerState.Laying)
        {
            survivalStatsManager.EnterTent(); // 텐트 회복 속도로 설정
        }
        else
        {
            survivalStatsManager.ExitTent();
        }

        // Stun 상태 설정
        if (currentState == PlayerState.Stun)
        {
            isStunned = true;
            playerMovement.enabled = false;
        }
        else
        {
            isStunned = false;
        }
    }

    /// <summary>
    /// Plays the appropriate animation based on the player's current state.
    /// </summary>
    void PlayerAnimationPlay()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                playerAnimator.PlayWalkingAnimation(false); // Idle 상태에서는 걷기 애니메이션 해제
                playerMovement.enabled = true; // Idle 상태에서는 이동 가능
                break;
            case PlayerState.Walking:
                playerAnimator.PlayWalkingAnimation(true);
                break;
            case PlayerState.Sitting:
                playerAnimator.PlaySitAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Laying:
                playerAnimator.PlayLayingAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Working:
                playerAnimator.PlayWorkingAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Stun:
                playerAnimator.PlayerStunAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                break;
            case PlayerState.Cooking:
                playerAnimator.PlayCookingAnimation();
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                currentState = PlayerState.Motion;
                break;
            case PlayerState.Motion:
                // Motion 상태에서는 PickUp, Eat, Drink 등의 행동을 재생
                break;
            case PlayerState.Death:
                playerAnimator.PlayerStunAnimation(true);
                playerMovement.enabled = false; // 다른 상태에서는 이동 불가
                currentState = PlayerState.Motion;
                break;
            case PlayerState.Ending:
                playerAnimator.PlayEndingMotion();
                break;
        }
    }


    public void PickUp()
    {
        currentState = PlayerState.Motion;
        playerAnimator.PlayPickUpAnimation();
        playerMovement.enabled = false; // 다른 상태에서는 이동 불가
    }

    public void Eat()
    {
        if (currentState != PlayerState.Laying || currentState != PlayerState.Stun)
        {
            currentState = PlayerState.Motion;
            playerAnimator.EatingAnimation();
            playerMovement.enabled = false; // 다른 상태에서는 이동 불가
        }
    }

    public void Drink()
    {
        if (currentState != PlayerState.Laying || currentState != PlayerState.Stun)
        {
            currentState = PlayerState.Motion;
            playerAnimator.DrinkingAnimation();
            playerMovement.enabled = false; // 다른 상태에서는 이동 불가
        }
    }

    /// <summary>
    /// Returns the current player state.
    /// </summary>
    /// <returns>The current player state.</returns>
    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    void TentInteraction()
    {
        if (tentTrans == null || isStunned || currentState == PlayerState.Laying || currentState == PlayerState.Motion) // Stun 상태일 때는 상호작용 불가
            return;

        InteractionUIManager.instance.DisableUIForObject(tentTrans);

        MoveToTentCenter();
        GameManager.instance.PlayerRecovery();
        GetComponent<TentPlacement>().BackpackSetActive(true);
    }

    public void WakeUp()
    {
        ReturnToOriginalPosition();
        GetComponent<TentPlacement>().BackpackSetActive(false);
        InteractionUIManager.instance.AddInteractionObject(tentTrans, "R", "Laying");
    }

    public void StunRecovery()
    {
        bool isGameOver = GameManager.instance.IsGameOver;
        if (!isGameOver)
        {
            isStunned = false; // Stun 상태 해제
            playerAnimator.PlayerStunAnimation(false); // Stun 해제 애니메이션 재생
            SetState(PlayerState.Idle); // Stun 해제 후 Idle 상태로 전환 (필요시 다른 상태로 변경 가능)
        }
    }

    IEnumerator TentInteractionCooldownCoroutine()
    {
        yield return new WaitForSeconds(tentInteractionCooldown);
        canInteractWithTent = true;
    }
    public void WorkEnd()
    {
        currentState = PlayerState.Motion;
        playerAnimator.PlayWorkingAnimation(false);
        playerMovement.enabled = true;
    }

    public void Ending()
    {
        currentState = PlayerState.Ending;
        playerAnimator.PlayEndingMotion();
        playerMovement.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Laying"))
        {
            if (tentTrans == null)
            {
                tentTrans = other.transform;
                SetTentTransform(other.transform);
                InputManager.instance.OnLayingKeyPressed += TentInteraction;
                InteractionUIManager.instance.AddInteractionObject(tentTrans, "R", "Laying");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Laying") && tentTrans == other.transform)
        {
            InputManager.instance.OnLayingKeyPressed -= TentInteraction;
            if (tentTrans != null)
            {
                InteractionUIManager.instance.DisableUIForObject(tentTrans);
            }
            tentTrans = null;
        }
    }
}
