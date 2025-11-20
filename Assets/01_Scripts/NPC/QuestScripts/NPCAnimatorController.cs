using UnityEngine;

public class NPCAnimatorController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    // 애니메이터 파라미터 이름들 (여기서는 예제 이름을 사용했지만 필요에 따라 변경)
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsIdle = Animator.StringToHash("isIdle");

    // 랜덤 행동 트리거
    private static readonly int RandomAct = Animator.StringToHash("randomAct");

    private float randomActTimer; // 랜덤 행동을 위한 타이머
    public float randomActInterval = 5f; // 랜덤 행동의 최소 간격

    void Start()
    {
        // 초기 상태를 Idle로 설정
        SetIdle();
    }

    void Update()
    {
        if (animator.GetBool(IsIdle))
        {
            randomActTimer += Time.deltaTime;
            if (randomActTimer >= randomActInterval)
            {
                TryRandomAction();
                randomActTimer = 0f; // 타이머 초기화
            }
        }
    }

    private void FixedUpdate()
    {
        animator.transform.localPosition = Vector3.zero;
        animator.transform.localRotation = Quaternion.identity;
    }

    // 걷기 애니메이션을 시작합니다.
    public void SetWalking()
    {
        animator.SetBool(IsWalking, true);
        animator.SetBool(IsRunning, false);
        animator.SetBool(IsIdle, false);
    }

    // 달리기 애니메이션을 시작합니다.
    public void SetRunning()
    {
        animator.SetBool(IsRunning, true);
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsIdle, false);
    }

    // 대기 애니메이션을 시작합니다.
    public void SetIdle()
    {
        animator.SetBool(IsIdle, true);
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsRunning, false);
    }

    // 특정 애니메이션을 멈추고 대기 상태로 전환합니다.
    public void StopAnimation()
    {
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsRunning, false);
        animator.SetBool(IsIdle, true);
    }

    // 랜덤 행동을 시도합니다.
    private void TryRandomAction()
    {
        float randomValue = Random.value;

        if (randomValue < 0.3f)
        {
            animator.SetTrigger(RandomAct); // 30% 확률로 randomAct 실행
        }
    }

    // 외부에서 랜덤 행동을 트리거할 수 있도록 하는 메서드
    public void TriggerRandomAct()
    {
        TryRandomAction();
    }
}
