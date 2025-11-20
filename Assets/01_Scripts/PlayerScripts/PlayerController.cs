using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public variables
    [Header("Stats")]
    public SurvivalStats survivalStats;

    [Header("Movement")]
    public float moveSpeed = 5f; // 기본 이동 속도
    public float reducedMoveSpeed1 = 3f; // 첫 번째 감소된 이동 속도
    public float reducedMoveSpeed2 = 1f; // 두 번째 감소된 이동 속도
    public float healthThreshold1 = 50f; // 첫 번째 임계값
    public float healthThreshold2 = 20f; // 두 번째 임계값
    public float turnSpeed = 360f; // 회전 속도
    public Transform cameraTransform; // 카메라의 Transform

    [Header("Rotation")]
    public float rotationSpeed = 100f; // 회전 속도
    public float minRotation = -45f;   // 최소 회전 각도
    public float maxRotation = 45f;    // 최대 회전 각도
    public bool shouldLimitRotation = true; // 회전 각도 제한 여부

    [Header("Inventory")]
    //public List<Item> inventory = new List<Item>();

    [Header("Tent Setup")]
    public GameObject tentPrefab; // 텐트 프리팹
    public LayerMask groundLayer; // 텐트를 설치할 수 있는 레이어
    public float maxSlope = 30f; // 최대 경사도

    // Private variables
    private Rigidbody rb;
    private PlayerAnimator playerAnimator;
    private float horizontalInput;
    private float verticalInput;
    private float currentRotation;
    private PlayerState currentState = PlayerState.Idle;
    private bool isPlacingTent = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트 유지
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<PlayerAnimator>();
        transform.eulerAngles = new Vector3(0, 0, 0); // 초기 회전 각도 설정
        currentRotation = transform.eulerAngles.y; // 초기 회전 각도 설정
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 잠금
    }

    void Update()
    {
        UpdateStats();
        GetInput();
        HandleState();

        if (isPlacingTent)
        {
            HandleTentPlacement();
        }
    }

    void UpdateStats()
    {
        survivalStats.UpdateStats(Time.deltaTime);
        UIManager.instance.UpdateSurvivalStats(survivalStats);
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A, D 키로 좌우 이동
        verticalInput = Input.GetAxisRaw("Vertical"); // W, S 키로 전진 및 후진
    }

    void HandleState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Walking:
                MoveCharacter();
                RotateCharacter();
                playerAnimator.PlayWalkingAnimation(verticalInput != 0);
                break;
            case PlayerState.Sitting:
                HandleSittingState();
                break;
            case PlayerState.Laying:
                HandleLayingState();
                break;
            case PlayerState.Working:
                HandleWorkingState();
                break;
            case PlayerState.PickingUp:
                playerAnimator.PlayPickUpAnimation();
                SetState(PlayerState.Idle);
                break;
        }
    }

    void HandleTentPlacement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTent();
        }
    }

    void TryPlaceTent()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 normal = hit.normal;
            float slope = Vector3.Angle(normal, Vector3.up);

            if (slope <= maxSlope)
            {
                PlaceTent(hit.point);
            }
            else
            {
                Debug.Log("Slope is too steep to place the tent.");
            }
        }
    }

    void PlaceTent(Vector3 position)
    {
        Instantiate(tentPrefab, position, Quaternion.identity);
        Debug.Log("Tent placed at " + position);
        isPlacingTent = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void MoveCharacter()
    {
        Vector3 moveDirection = new Vector3(0, 0, verticalInput).normalized;

        // 체력이 임계값 이하일 때 이동 속도를 감소시킴
        float currentMoveSpeed = moveSpeed;
        if (survivalStats.health <= healthThreshold2)
        {
            currentMoveSpeed = reducedMoveSpeed2;
        }
        else if (survivalStats.health <= healthThreshold1)
        {
            currentMoveSpeed = reducedMoveSpeed1;
        }

        playerAnimator.SetWalkingSpeed(currentMoveSpeed / moveSpeed); // 애니메이션 속도 조절

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            rb.velocity = moveDir * currentMoveSpeed;
            SetState(PlayerState.Walking);
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            SetState(PlayerState.Idle);
        }
    }

    void RotateCharacter()
    {
        if (horizontalInput != 0)
        {
            currentRotation += horizontalInput * rotationSpeed * Time.deltaTime;

            if (shouldLimitRotation)
            {
                currentRotation = Mathf.Clamp(currentRotation, minRotation, maxRotation); // 회전 각도 제한
            }

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);
        }
    }

    void HandleSittingState()
    {
        playerAnimator.PlaySitAnimation(true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void HandleLayingState()
    {
        playerAnimator.PlayLayingAnimation(true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void HandleWorkingState()
    {
        playerAnimator.PlayWorkingAnimation(true);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void SetState(PlayerState newState)
    {
        currentState = newState;

        // 상태에 따른 애니메이션 설정
        playerAnimator.PlaySitAnimation(currentState == PlayerState.Sitting);
        playerAnimator.PlayLayingAnimation(currentState == PlayerState.Laying);
        playerAnimator.PlayWorkingAnimation(currentState == PlayerState.Working);
    }

    void OnTriggerEnter(Collider other)
    {
        // 아이템을 수집할 때
        ItemPickup itemPickup = other.GetComponent<ItemPickup>();
        if (itemPickup != null)
        {
            InventoryManager.instance.AddItem(itemPickup.items);
            Destroy(other.gameObject); // 아이템 오브젝트 삭제
        }
    }
}
