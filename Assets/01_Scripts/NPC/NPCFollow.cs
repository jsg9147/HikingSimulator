using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCFollow : MonoBehaviour
{
    public Animator animator;

    public float followDistance = 2.0f;  // 플레이어와 NPC 간의 거리
    public float moveSpeed = 5.0f;  // NPC의 이동 속도
    public float stoppingDistance = 1.5f;  // 멈추는 거리
    public float rotationSpeed = 5.0f;  // 회전 속도

    private Transform player;  // 플레이어의 위치를 받아오기 위한 변수
    private Rigidbody rb;

    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameManager.instance.playerTransfrom;
        isMoving = false;
        DontDestroyOnLoad(gameObject);
    }

    void FixedUpdate()
    {
        animator.transform.localPosition = Vector3.zero;

        if (isMoving)
        {
            // 플레이어와 NPC 간의 거리 계산
            float distance = Vector3.Distance(player.position, transform.position);

            // NPC가 플레이어를 따라가야 하는지 확인
            if (distance > stoppingDistance)
            {
                // 플레이어를 향해 회전
                Vector3 direction = (player.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * rotationSpeed);

                // 플레이어를 향해 이동
                Vector3 move = transform.forward * moveSpeed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + move);
                animator.SetBool("isWalking", true); // 걷기 애니메이션 시작
            }
            else
            {
                // 플레이어와 너무 가까운 경우 멈춤
                rb.velocity = Vector3.zero;
                animator.SetBool("isWalking", false); // 걷기 애니메이션 시작
            }
        }
    }

    public void MoveStart() => isMoving = true;
    public void MoveStop() => isMoving = false;

    private void OnDisable()
    {
        if (!isMoving)
            Destroy(gameObject);
    }
}
