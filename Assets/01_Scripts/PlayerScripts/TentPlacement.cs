using System.Collections;
using UnityEngine;

public class TentPlacement : MonoBehaviour
{
    public TentItem tentPrefab; // 텐트 프리팹
    public LayerMask groundLayer; // 텐트를 설치할 수 있는 레이어
    public float maxDistance = 30f; // 텐트 설치 최대 거리
    public KeyCode placementKey = KeyCode.T; // 텐트 배치 모드 활성화 키

    private TentItem tentPreviewInstance; // 텐트 프리뷰 인스턴스
    private bool isPlacingTent = false; // 텐트 설치 모드 여부
    private bool canPlaceTent = false; // 텐트 설치 가능 여부

    private Renderer tentRenderer; // 텐트의 렌더러
    private ItemData tentData;
    private TentItem placedTent = null; // 이미 설치된 텐트 인스턴스

    private Vector3 setupPos;
    private Quaternion setupRotation;


    void Update()
    {
        if (isPlacingTent)
        {
            UpdateTentPreviewPosition();

            if (Input.GetMouseButtonDown(0) && canPlaceTent)
            {
                Vector3 newRotation = tentPreviewInstance.tentPreviewRenderer.transform.rotation.eulerAngles;

                // y축 회전에 180도 더함
                newRotation.y += 180f;

                setupPos = tentPreviewInstance.tentPreviewRenderer.transform.position;
                setupRotation = Quaternion.Euler(newRotation);
                GameManager.instance.playerMovement.MoveToPosition(setupPos + (Vector3.back * 5f));
                Destroy(tentPreviewInstance.gameObject);
                isPlacingTent = false;
                UIManager.instance.CursorChange(CursorLockMode.Locked); 
            }
            else if (Input.GetMouseButtonDown(1))
            {
                ToggleTentPlacementMode(tentData);
            }
        }
    }

    public void ToggleTentPlacementMode(ItemData tentData)
    {
        if (placedTent != null)
        {
            Debug.Log("Tent is already placed.");
            return;
        }

        if (!isPlacingTent)
        {
            UIManager.instance.CursorChange(CursorLockMode.None);
            tentPreviewInstance = Instantiate(tentPrefab);
            tentPreviewInstance.gameObject.SetActive(true);
            tentRenderer = tentPreviewInstance.tentPreviewRenderer;
            tentPreviewInstance.TentColliderEnable(false);
            tentPreviewInstance.transform.position = transform.position + (Vector3.forward * 10f);
            isPlacingTent = true;
            this.tentData = tentData;
        }
        else
        {
            UIManager.instance.CursorChange(CursorLockMode.Locked);
            Destroy(tentPreviewInstance.gameObject);
            isPlacingTent = false;
        }
    }
    void UpdateTentPreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance, groundLayer))
        {
            tentPreviewInstance.transform.position = hit.point;
            tentPreviewInstance.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (hit.collider.isTrigger)
            {
                if (hit.collider.CompareTag("TentZone"))
                {
                    tentRenderer.material.SetColor("_BaseColor", Color.green); // 설치 가능 색상
                    canPlaceTent = true;

                    // 현재 충돌한 객체의 회전을 가져와서 Euler 각도로 변환
                    Vector3 newRotation = hit.collider.transform.rotation.eulerAngles;

                    // y축 회전에 180도 더함
                    newRotation.y += 180f;

                    // 수정된 회전을 tentRenderer의 회전으로 설정
                    tentRenderer.transform.rotation = Quaternion.Euler(newRotation);
                }
                else
                {
                    tentRenderer.material.SetColor("_BaseColor", Color.red); // 설치 불가 색상
                    canPlaceTent = false;
                }
            }
            else
            {
                if (hit.collider.CompareTag("TentZone"))
                {
                    // 현재 충돌한 객체의 회전을 가져와서 Euler 각도로 변환
                    Vector3 newRotation = hit.collider.transform.rotation.eulerAngles;

                    // y축 회전에 180도 더함
                    newRotation.y += 180f;
                    tentRenderer.material.SetColor("_BaseColor", Color.green); // 설치 가능 색상
                    tentRenderer.transform.rotation = Quaternion.Euler(newRotation);
                    canPlaceTent = true;
                }
                else
                {
                    tentRenderer.material.SetColor("_BaseColor", Color.red); // 설치 불가 색상
                    canPlaceTent = false;
                }
            }
        }
    }


    public void PlaceTent(Vector3 position, Quaternion rotation)
    {
        if (placedTent != null)
        {
            Debug.Log("Tent is already placed.");
            return;
        }

        TentItem newTent = Instantiate(tentPrefab);
        newTent.transform.position = position;
        newTent.transform.rotation = rotation;
        newTent.tag = "Tent"; // 새로 생성된 텐트에 "Tent" 태그 설정
        newTent.TentColliderEnable(true);
        placedTent = newTent; // 새로 생성된 텐트를 변수에 할당
        InventoryManager.instance.RemoveItem(tentData);
        SurvivalStatsManager.instance.IncrementTentSetupCount(); // 텐트 설치 횟수 증가
    }

    public void UseTentItem()
    {
        if (canPlaceTent)
        {
            GameManager.instance.playerStateController.SetState(PlayerState.Working);
            InventoryManager.instance.inventory.RemoveItem(tentData);
            InventoryManager.instance.UpdateInventoryUI();
            StartCoroutine(PlaceTentAfterDelay(3f));
        }
        else
        {
            Debug.Log("I can't place the tent in this zone");
        }
    }

    IEnumerator PlaceTentAfterDelay(float delay)
    {
        UIManager.instance.ProgressBar(delay);
        yield return new WaitForSeconds(delay);
        PlaceTent(setupPos, setupRotation);
        GameManager.instance.playerStateController.WorkEnd();
        isPlacingTent = false;
    }

    public void TakeDownTent()
    {
        if (placedTent != null && InventoryManager.instance.AddItem(tentData))
        {
            InteractionUIManager.instance.DisableUIForObject(placedTent.transform);

            Destroy(placedTent.gameObject);
            placedTent = null;
            isPlacingTent = false;
            tentData = null;
            InputManager.instance.OnInteractKeyPressed -= TakeDown;
        }
    }

    IEnumerator TakeDownTentAfterDelay(float delay)
    {
        GameManager.instance.playerStateController.SetState(PlayerState.Working);
        UIManager.instance.ProgressBar(delay);
        yield return new WaitForSeconds(delay);
        GameManager.instance.playerStateController.WorkEnd();
        
        TakeDownTent();
    }

    void TakeDown()
    {
        if (GameManager.instance.playerStateController.CurrentState == PlayerState.Motion)
            return;

        StartCoroutine(TakeDownTentAfterDelay(3f));
    }

    public void BackpackSetActive(bool isActive)
    {
        placedTent.backpack.SetActive(isActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tent"))
        {
            InputManager.instance.OnInteractKeyPressed += TakeDown;
            InteractionUIManager.instance.AddInteractionObject(other.transform, "E", "Take down");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tent"))
        {
            InputManager.instance.OnInteractKeyPressed -= TakeDown;
            InteractionUIManager.instance.DisableUIForObject(other.transform);
        }
    }
}
