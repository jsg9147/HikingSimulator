using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NPCAnimatorController))]
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(CapsuleCollider))]
public class RandomPathMover : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public Transform[] points; // 경로의 포인트들
    }

    public List<Path> paths = new List<Path>(); // 여러 경로를 담는 리스트
    public float moveSpeed = 2f; // 이동 속도
    public float rotationSpeed = 5f; // 회전 속도
    public bool reverseMovement = false; // 역순 이동 여부 (인스펙터에서 제어)
    public bool isLooping = true; // 반복 이동 여부

    public float minPauseTime = 1f; // 최소 멈춤 시간
    public float maxPauseTime = 3f; // 최대 멈춤 시간
    private float pauseTimer; // 멈춤 타이머
    private float nextPauseTime; // 다음 멈춤 시간

    public float detectionRange = 30f; // 플레이어와의 거리 임계값

    private NPCAnimatorController animatorController;
    private Transform[] currentPath; // 현재 선택된 경로의 포인트들
    private int currentPathIndex = 0; // 경로 리스트에서 현재 경로 인덱스
    private int currentPointIndex = 0; // 경로 내 현재 포인트 인덱스
    private bool moving = false; // 이동 중인지 여부
    private bool reverse = false; // 내부에서 역순 이동 여부를 판단하는 변수
    private Rigidbody rb; // 리지드바디

    private bool nearPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        animatorController = GetComponent<NPCAnimatorController>();
        ChoosePathIncludingCurrentPosition();
        SetNextPauseTime();

        animatorController.SetIdle();
    }

    void Update()
    {
        if (moving)
        {
            MoveAlongPath();
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= nextPauseTime)
            {
                StopMoving();
                Invoke("ResumeMoving", Random.Range(minPauseTime, maxPauseTime)); // 일정 시간 후 다시 이동
                SetNextPauseTime(); // 다음 멈춤 시간을 설정
            }
        }

        // 플레이어와의 거리를 체크하여 이동 여부를 결정
        PlayerPosCheck();

        if (nearPlayer && !moving)
        {
            StartMoving();
        }
    }

    void PlayerPosCheck()
    {
        if (GameManager.instance.playerTransfrom == null) return;

        // 플레이어와의 거리를 계산
        float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.playerTransfrom.position);

        // 플레이어가 특정 거리 내로 들어왔는지 확인
        if (distanceToPlayer <= detectionRange)
        {
            nearPlayer = true; // 플레이어가 근처에 있음
        }
        else
        {
            nearPlayer = false; // 플레이어가 멀리 있음
        }
    }

    void SetNextPauseTime()
    {
        pauseTimer = 0f;
        nextPauseTime = Random.Range(3f, 10f); // 3초에서 10초 사이의 랜덤한 시간
    }

    void ChoosePathIncludingCurrentPosition()
    {
        foreach (var path in paths)
        {
            int closestPointIndex = GetClosestPointIndexInPath(path.points);
            if (closestPointIndex != -1)
            {
                currentPath = path.points;
                reverse = reverseMovement;
                currentPointIndex = closestPointIndex;
                return;
            }
        }

        ChooseRandomPath();
    }

    int GetClosestPointIndexInPath(Transform[] pathPoints)
    {
        float closestDistance = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < pathPoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, pathPoints[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        if (closestDistance < 0.1f)
        {
            return closestIndex;
        }

        return -1;
    }

    void ChooseRandomPath()
    {
        currentPathIndex = Random.Range(0, paths.Count);
        currentPath = paths[currentPathIndex].points;
        reverse = reverseMovement;
        currentPointIndex = reverse ? currentPath.Length - 1 : 0;
    }

    void StartMoving()
    {
        if (currentPath.Length > 0)
        {
            moving = true;
            animatorController.SetWalking(); // 걷기 애니메이션 시작
        }
    }
    void MoveAlongPath()
    {
        if (currentPath.Length == 0) return;
        Transform targetPoint = currentPath[currentPointIndex];

        // y 값을 무시한 방향 벡터 계산
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        direction.y = 0; // y 축의 차이를 무시

        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 회전 차이 각도 계산
        float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);

        // 일정 각도 이하로 가까워지면 회전을 멈추고 바로 타겟 방향으로 설정
        if (angleToTarget > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = targetRotation; // 각도가 작으면 목표 회전에 맞춰 바로 정렬
        }

        // Rigidbody를 사용하여 이동
        Vector3 newPosition = rb.position + direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(newPosition);

        // y축을 무시한 거리 계산
        Vector3 flatPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatTarget = new Vector3(targetPoint.position.x, 0, targetPoint.position.z);

        // 지점에 도착했는지 체크 (거리가 1 미만일 때)
        if (Vector3.Distance(flatPosition, flatTarget) < 1f) // y축을 무시한 거리 계산
        {
            if (reverse)
            {
                currentPointIndex--;
                if (currentPointIndex < 0)
                {
                    if (isLooping)
                    {
                        reverse = false; // 방향 변경
                        currentPointIndex = 0; // 경로의 시작으로 이동
                    }
                    else
                    {
                        StopAndDisappear();
                    }
                }
            }
            else
            {
                currentPointIndex++;
                if (currentPointIndex >= currentPath.Length)
                {
                    if (isLooping)
                    {
                        reverse = true; // 방향 변경
                        currentPointIndex = currentPath.Length - 1; // 경로의 끝으로 이동
                    }
                    else
                    {
                        StopAndDisappear();
                    }
                }
            }
        }
    }


    void StopAndDisappear()
    {
        moving = false;
        animatorController.StopAnimation(); // 애니메이션 멈춤
        Destroy(gameObject); // 오브젝트 제거
    }

    public void StopMoving()
    {
        moving = false;
        animatorController.StopAnimation(); // 애니메이션 멈춤
    }

    public void ResumeMoving()
    {
        if (currentPath != null && currentPath.Length > 0)
        {
            moving = true;
            animatorController.SetWalking(); // 걷기 애니메이션 다시 시작
        }
    }
}
