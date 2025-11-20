using System.Collections;
using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    private const int Idle = 0;
    private const int Walk = 1;
    private const int Run = 2;
    private const int Jump = 3;
    private const int Eat = 4;
    private const int Rest = 5;

    public float speed = 5.0f; // 기본 이동 속도
    public float jumpForce = 3f; // 점프할 때 위로 가하는 힘
    public float minChangeInterval = 1.0f; // 최소 행동 변경 간격 (초 단위)
    public float maxChangeInterval = 5.0f; // 최대 행동 변경 간격 (초 단위)

    public float detectionRange = 10f; // 목표 오브젝트 감지 범위
    public float jumpDistance = 2f; // 목표 오브젝트에 가까워졌을 때 점프하는 거리
    public float chargeProbability = 0.3f; // 돌진할 확률 (0.0 ~ 1.0)

    private Animator animator; // Animator 컴포넌트
    private Rigidbody rb; // Rigidbody 컴포넌트
    private Vector3 targetDirection;
    private Transform playerTransform; // 추적할 목표 오브젝트

    public LayerMask groundLayer; // 바닥 레이어를 설정할 수 있는 옵션
    public LayerMask wallLayer; // 벽을 감지할 레이어

    private bool collidedWithWall = false; // 벽에 충돌했는지 여부

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        // Rigidbody 회전 제약 설정
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (GameManager.instance != null)
        {
            playerTransform = GameManager.instance.playerTransfrom;
        }

        // 시작할 때 초기 방향 설정
        targetDirection = GetRandomDirection();
        // 행동을 주기적으로 변경하는 코루틴 시작
        StartCoroutine(ChangeActionRoutine());
    }

    void FixedUpdate()
    {
        // 이동 애니메이션 상태일 때만 이동 및 회전
        if (!collidedWithWall && (animator.GetInteger("animation") == Walk || animator.GetInteger("animation") == Run || animator.GetInteger("animation") == Jump))
        {
            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                rb.MoveRotation(targetRotation);
            }

            // 바닥의 경사에 따라 X축 회전 조정
            AdjustRotationToSlope();

            rb.MovePosition(rb.position + targetDirection * speed * Time.fixedDeltaTime);

            // 점프 상태일 때 위로 힘을 가함
            if (animator.GetInteger("animation") == Jump)
            {
                rb.AddForce(Vector3.up * jumpForce);
            }
        }
    }

    void AdjustRotationToSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
        {
            // 바닥의 법선 벡터를 얻고, 오브젝트의 회전 각도 계산
            Vector3 groundNormal = hit.normal;
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);

            // 현재 회전값을 유지하면서 X축 회전만 수정
            Quaternion targetRotation = Quaternion.Euler(slopeRotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 부드럽게 회전
        }
    }

    Vector3 GetRandomDirection()
    {
        // 랜덤한 방향을 반환
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    IEnumerator ChangeActionRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minChangeInterval, maxChangeInterval);
            yield return new WaitForSeconds(waitTime);

            if (animator.GetInteger("animation") == Idle)
            {
                int action = Random.Range(1, 6); // 1부터 5까지 선택 가능
                switch (action)
                {
                    case Walk:
                        animator.SetInteger("animation", Walk);
                        targetDirection = GetRandomDirection();
                        speed = 3.0f;
                        break;
                    case Run:
                        animator.SetInteger("animation", Run);
                        targetDirection = GetRandomDirection();
                        speed = 6.0f;
                        break;
                    case Jump:
                        animator.SetInteger("animation", Jump);
                        targetDirection = GetRandomDirection();
                        speed = 10f;
                        break;
                    case Eat:
                        animator.SetInteger("animation", Eat);
                        targetDirection = Vector3.zero;
                        break;
                    case Rest:
                        animator.SetInteger("animation", Rest);
                        targetDirection = Vector3.zero;
                        break;
                }

                // 돌진 행동 결정
                if (playerTransform != null && Random.value < chargeProbability)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);

                    // 목표 오브젝트가 감지 범위 내에 있을 때 Run 상태로 전환
                    if (distanceToTarget <= detectionRange)
                    {
                        targetDirection = (playerTransform.position - transform.position).normalized;
                        animator.SetInteger("animation", Run);
                        speed = 6.0f;

                        // 목표 오브젝트에 충분히 가까워지면 Jump 상태로 전환
                        while (distanceToTarget > jumpDistance)
                        {
                            distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
                            yield return null;
                        }

                        animator.SetInteger("animation", Jump);
                        rb.AddForce(Vector3.up * jumpForce);
                        speed = 10f;
                    }
                }

                yield return new WaitForSeconds(Random.Range(minChangeInterval, maxChangeInterval));
                animator.SetInteger("animation", Idle);
                targetDirection = Vector3.zero;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (speed >= 10f)
                GameManager.instance.playerStateController.SetState(PlayerState.Stun);
        }

        // 벽에 부딪혔을 때 충돌 감지
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            collidedWithWall = true;
            targetDirection = Vector3.zero; // 이동 중단
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 벽에서 떨어졌을 때 다시 이동 가능하게
        if (((1 << collision.gameObject.layer) & wallLayer) != 0)
        {
            collidedWithWall = false;
            targetDirection = GetRandomDirection(); // 새로운 방향 설정
        }
    }
}
