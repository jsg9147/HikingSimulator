using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonBase<CameraController>
{
    private float Yaxis;
    private float Xaxis;
    public Transform target; // 플레이어의 Transform 참조

    public Transform secondCamera;

    public float rotSensitive = 3f; // 카메라 회전 감도
    public float dis = 2f; // 기본 플레이어와의 거리
    public float yDis = 2f; // 플레이어와의 수직 거리
    public float RotationMin = -10f; // 카메라의 최소 피치 각도
    public float RotationMax = 80f; // 카메라의 최대 피치 각도
    public float smoothTime = 0.12f; // 회전에 소요되는 시간

    public float minYRotation = -90f; // Y축 최소 회전 각도
    public float maxYRotation = 90f; // Y축 최대 회전 각도
    public float distanceThreshold = 0.1f; // 거리 변화에 대한 임계값
    public float distanceLerpSpeed = 5f; // 거리 변화 속도

    private Camera mainCamera;

    private Vector3 targetRotation;
    private Vector3 currentVel;
    private float currentDis;

    // Transition 관련 변수 추가
    private bool isNight = false; // 낮/밤 상태를 추적하는 플래그
    public float transitionDuration = 5f; // 전환 지속 시간
    private bool isTransitioning = false; // 전환 중인지 추적

    // 카메라 조작 활성화 여부 플래그
    public bool isCameraControlEnabled = true;
    private void Start()
    {
        currentDis = dis;
    }

    void LateUpdate() // 플레이어의 움직임 후 카메라 업데이트
    {
        if (!isCameraControlEnabled)
            return; // 카메라 조작이 비활성화된 상태면 아무것도 하지 않음

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // 마우스 입력에 따라 카메라 회전 조정
            Yaxis += Input.GetAxis("Mouse X") * rotSensitive;
            Xaxis -= Input.GetAxis("Mouse Y") * rotSensitive;
        }

        // X축과 Y축 회전 각도를 제한
        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);

        // 목표 회전 각도로 부드럽게 전환
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;

        // 장애물에 따라 카메라 거리 조정
        AdjustCameraDistance();

        // 플레이어 위치를 기준으로 카메라 위치 업데이트
        transform.position = target.position - (transform.forward * currentDis) + (Vector3.up * yDis);
    }

    private void AdjustCameraDistance()
    {
        RaycastHit hit;
        Vector3 cameraPosition = target.position - (transform.forward * dis) + (Vector3.up * yDis); // 장애물이 없을 때 카메라 위치
        Vector3 rayDirection = cameraPosition - target.position; // 플레이어에서 카메라로의 벡터 방향
        float targetHitDistance = dis; // 타겟에서 레이캐스트를 위한 기본 거리

        // "Player" 레이어를 무시하기 위한 레이어 마스크 생성
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        layerMask = ~layerMask; // 반전하여 "Player" 레이어를 무시

        // 플레이어 위치에서 레이캐스트
        if (Physics.Raycast(target.position, rayDirection, out hit, dis, layerMask))
        {
            if (hit.collider != null)
            {
                if(hit.collider.CompareTag("Building"))
                {
                    targetHitDistance = hit.distance;

                    // 장애물이 감지되면 카메라를 플레이어에 최대한 가깝게 붙이기
                    currentDis = Mathf.Lerp(currentDis, targetHitDistance, Time.deltaTime * distanceLerpSpeed);
                }
            }
        }
        else
        {
            // 장애물이 없을 때 기본 거리로 설정
            currentDis = Mathf.Lerp(currentDis, dis, Time.deltaTime * distanceLerpSpeed);
        }

        // 일정 임계값 이하의 변화는 무시하여 카메라가 너무 자주 움직이지 않도록 설정
        if (Mathf.Abs(currentDis - dis) < distanceThreshold)
        {
            currentDis = dis;
        }
    }

    public void CameraChange()
    {
        secondCamera.gameObject.SetActive(true);
        secondCamera.transform.position = transform.position;
        secondCamera.rotation = transform.rotation;
        secondCamera.SetParent(null);

        GetComponent<Camera>().gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        //Destroy(secondCamera.gameObject);
    }

    private IEnumerator AdjustCameraForNight()
    {
        float initialClipPlane = GetComponent<Camera>().farClipPlane;
        float targetClipPlane = 250f; // 밤에 가시거리 제한

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float currentClipPlane = Mathf.Lerp(initialClipPlane, targetClipPlane, elapsedTime / transitionDuration);
            GetComponent<Camera>().farClipPlane = currentClipPlane;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GetComponent<Camera>().farClipPlane = targetClipPlane;
        isTransitioning = false;
    }

    private IEnumerator ResetCameraView()
    {
        float initialClipPlane = GetComponent<Camera>().farClipPlane;
        float targetClipPlane = 2000f; // 낮에 기본 가시거리로 복원

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float currentClipPlane = Mathf.Lerp(initialClipPlane, targetClipPlane, elapsedTime / transitionDuration);
            GetComponent<Camera>().farClipPlane = currentClipPlane;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GetComponent<Camera>().farClipPlane = targetClipPlane;
        isTransitioning = false;
    }

    // 낮과 밤을 전환하는 메서드 추가
    public void ToggleDayNight(bool night)
    {
        if (isNight != night && !isTransitioning) // 상태가 변할 때만 실행
        {
            isNight = night;
            isTransitioning = true;

            if (isNight)
            {
                StartCoroutine(AdjustCameraForNight());
            }
            else
            {
                StartCoroutine(ResetCameraView());
            }
        }
    }

    public void SetMainCamera()
    {
        if (Camera.main != GetComponent<Camera>())
        {
            Destroy(Camera.main.gameObject);
        }
    }

    public void EndingCameraEffect()
    {
        // 카메라 조작을 비활성화
        
        // 시작 위치와 목표 위치
        Vector3 startPos = new Vector3(0, 4.5f, -4);
        Vector3 targetPos = new Vector3(0, 4.5f, -7.83f);

        // 시작 회전과 목표 회전
        Quaternion startRot = Quaternion.Euler(35, 0f, transform.rotation.eulerAngles.z);
        Quaternion targetRot = Quaternion.Euler(-66, 0f, transform.rotation.eulerAngles.z);

        // 카메라 이동과 회전을 서서히 실행하는 코루틴 실행
        StartCoroutine(MoveAndRotateCamera(startPos, targetPos, startRot, targetRot, 5f)); // 5초 동안 이동 및 회전
    }

    private IEnumerator MoveAndRotateCamera(Vector3 startPos, Vector3 targetPos, Quaternion startRot, Quaternion targetRot, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 위치와 회전을 서서히 변경
            transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            transform.localRotation = Quaternion.Lerp(startRot, targetRot, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null; // 한 프레임 대기
        }

        // 최종 위치와 회전 설정
        transform.localPosition = targetPos;
        transform.localRotation = targetRot;
    }

    public void SetEndingPos()
    {
        isCameraControlEnabled = false;
        transform.localPosition = new(0, 2.5f, -6f);
        transform.localRotation = Quaternion.identity;
    }
}
