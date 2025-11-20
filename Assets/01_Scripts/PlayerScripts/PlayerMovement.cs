using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform playerCharactor;
    public float basicSpeed = 4f;
    public float reducedMoveSpeedforInjury = 2f;
    public float rotationSpeed = 100f;
    public float stepHeight = 0.5f;
    public float jumpForce = 5f;
    public float minStepHeight = 0.1f; // 최소 높이 추가

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float horizontalInput;
    private float verticalInput;
    private float currentRotation;
    private float baseMoveSpeed;
    private bool isGrounded;
    private bool isMovingToTarget = false;
    private Vector3 targetPosition;
    private PlayerStateController playerStateController;
    private float moveSpeed;

    private bool isMovementBlocked = false;

    private bool isTouchingWall = false;
    public float wallFrictionMultiplier = 0.5f; // 벽에 닿았을 때 속도를 줄이는 비율
    public float wallBounceForce = 2f; // 벽에서 반발하는 힘

    // 이동한 거리를 기록하는 변수
    private float totalDistanceMoved = 0f;
    private Vector3 lastPosition;

    private Transform startPos;

    void Start()
    {
        moveSpeed = basicSpeed;
#if UNITY_EDITOR
        //moveSpeed = basicSpeed * 3f;
#endif
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentRotation = transform.eulerAngles.y;
        baseMoveSpeed = moveSpeed;
        playerStateController = GetComponent<PlayerStateController>();
        lastPosition = transform.position; // 초기 위치 설정
    }

    public void Init()
    {
        currentRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        if (GameManager.instance.IsGameOver)
            return;

        playerCharactor.transform.localPosition = Vector3.zero;
        playerCharactor.transform.localRotation = Quaternion.identity;

        if (isMovementBlocked) return;

        GetInput();
        if (!isMovingToTarget)
        {
            MoveCharacter();
            RotateCharacter();
        }
        else
        {
            MoveToTarget();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        // 이동 거리를 계산 및 업데이트
        UpdateDistanceMoved();
        CheckForFall();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            isMovingToTarget = false;
            playerStateController.SetState(PlayerState.Walking);
        }

        if (horizontalInput == 0 && verticalInput == 0 && !isMovingToTarget && playerStateController.GetCurrentState() != PlayerState.Working)
        {
            playerStateController.SetState(PlayerState.Idle);
        }
    }

    public void AdjustMoveSpeed(float currentHealth)
    {
        moveSpeed = baseMoveSpeed;

        if (SurvivalStatsManager.instance.injuryManager.injury == InjurySeverity.Major)
        {
            moveSpeed = reducedMoveSpeedforInjury;
        }

        if (WeatherManager.instance.currentWeather == WeatherType.Rain)
        {
            moveSpeed = moveSpeed * 0.8f;
        }
    }

    public float GetCurrentSpeedRatio()
    {
        return moveSpeed / baseMoveSpeed;
    }
    void CheckForFall()
    {
        // 현재 y 위치가 -100 이하로 떨어졌을 때
        if (transform.position.y < -100f)
        {
            // 낙하 속도 없애기 (Rigidbody의 속도 초기화)
            rb.velocity = Vector3.zero;

            // y 값을 50으로 설정하여 위치 조정
            Vector3 newPosition = transform.position;
            newPosition.y = 50f;
            rb.position = newPosition;

            // 추가로, 플레이어의 상태를 다시 Idle로 설정할 수도 있음
            playerStateController.SetState(PlayerState.Idle);
        }
    }


    void MoveCharacter()
    {
        Vector3 moveDirection = new Vector3(0, 0, verticalInput).normalized;
        Vector3 move = transform.forward * moveDirection.z * moveSpeed * Time.deltaTime;

        if (isTouchingWall)
        {
            // 벽에 닿았을 때 속도를 줄임
            move *= wallFrictionMultiplier;
        }

        Vector3 newPosition = rb.position + move;

        // 콜라이더의 발밑 부분에서 Ray 발사
        Vector3 rayOriginLow = rb.position + capsuleCollider.center - new Vector3(0, capsuleCollider.height / 2, 0) + transform.forward * capsuleCollider.radius;

        // 콜라이더의 minStepHeight 높이에서 Ray 발사
        Vector3 rayOriginMin = rayOriginLow + Vector3.up * minStepHeight;

        // 콜라이더의 stepHeight 높이에서 Ray 발사
        Vector3 rayOriginMax = rayOriginLow + Vector3.up * stepHeight;

        // 세 개의 Raycast를 발사
        bool hitLow = Physics.Raycast(rayOriginLow, transform.forward, out RaycastHit hitInfoLow, move.magnitude + 0.1f);
        bool hitMin = Physics.Raycast(rayOriginMin, transform.forward, out RaycastHit hitInfoMin, move.magnitude + 0.1f);
        bool hitMax = Physics.Raycast(rayOriginMax, transform.forward, out RaycastHit hitInfoMax, move.magnitude + 0.1f);

        // 장애물 처리 로직
        if (hitMax)
        {
            // 최대 높이에서 레이저가 감지되면 상승하지 않음
            newPosition.y = rb.position.y;
        }
        else if (hitLow && hitMin)
        {
            // 최소 높이와 가장 아래에서 레이저가 감지되면 최소 높이만큼 상승
            newPosition.y += minStepHeight;
        }
        else if (!hitLow && !hitMin)
        {
            // 최소 높이에서도 감지되지 않으면 높이 변경 없이 이동
            newPosition.y = rb.position.y;
        }

        rb.MovePosition(newPosition);
    }

    void RotateCharacter()
    {
        if (horizontalInput != 0)
        {
            currentRotation += horizontalInput * rotationSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotation, transform.eulerAngles.z);
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;

        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            isTouchingWall = true;

            // 충돌 각도를 계산해서, 일정 각도 이상일 때 반발력 추가
            Vector3 collisionNormal = collision.contacts[0].normal;
            float angle = Vector3.Angle(collisionNormal, transform.forward);

            if (angle > 45f) // 벽과 부딪힌 각도가 45도 이상일 때
            {
                Vector3 bounceDirection = Vector3.Reflect(transform.forward, collisionNormal).normalized;
                rb.AddForce(bounceDirection * wallBounceForce, ForceMode.Impulse);
                Init();
            }
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        targetPosition = position;
        isMovingToTarget = true;
    }

    void MoveToTarget()
    {
        // x와 z 축만 사용하는 평탄화된 목표 위치
        Vector3 flatTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 direction = (flatTargetPosition - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            toRotation = Quaternion.Euler(0, toRotation.eulerAngles.y, 0); // Y축만 회전하도록 설정
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }


        rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);

        // 거리 계산시 y축을 무시하고 x, z축만 사용
        Vector3 horizontalDistance = new Vector3(transform.position.x - flatTargetPosition.x, 0, transform.position.z - flatTargetPosition.z);

        if (horizontalDistance.magnitude < 0.1f)
        {
            isMovingToTarget = false;
            GameManager.instance.tentPlacement.UseTentItem();
        }
        else
        {
            playerStateController.SetState(PlayerState.Walking);
        }
    }


    public void SetMovementBlocked(bool blocked)
    {
        isMovementBlocked = blocked;

        if (blocked)
        {
            playerStateController.SetState(PlayerState.Idle);
        }
    }

    void UpdateDistanceMoved()
    {
        float distance = Vector3.Distance(lastPosition, transform.position);
        totalDistanceMoved += distance;
        lastPosition = transform.position;
    }

    public float GetTotalDistanceMoved()
    {
        return totalDistanceMoved / 2;
    }
}
