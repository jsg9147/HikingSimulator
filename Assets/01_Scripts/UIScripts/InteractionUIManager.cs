using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class InteractionUIManager : SingletonBase<InteractionUIManager>
{
    public Canvas canvas; // UI가 포함된 캔버스
    public InteractionUI interactionUIPrefab; // 상호작용 키를 표시할 UI 프리팹
    public int maxUIElements = 5; // UI 요소의 최대 수
    public float interactionDistance = 3f; // 상호작용이 가능한 거리

    private Transform player; // 플레이어의 Transform
    private List<Transform> targetObjects = new List<Transform>(); // 상호작용할 오브젝트들의 Transform 리스트
    private List<string> interactionKeys = new List<string>(); // 각 오브젝트에 대한 상호작용 키
    private List<string> descriptions = new List<string>(); // 각 오브젝트에 대한 설명
    private List<InteractionUI> activeUIInstances = new List<InteractionUI>(); // 활성화된 UI 인스턴스 리스트
    private Queue<InteractionUI> uiPool = new Queue<InteractionUI>(); // UI 재활용을 위한 풀

    void Start()
    {
        // 초기 UI 풀 생성
        Init();
    }

    void OnEnable()
    {
        // 씬이 로드될 때마다 Init을 호출
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 후 Init 호출
        Init();
    }

    void Update()
    {
        // 활성화된 UI 인스턴스의 위치를 갱신하고, 조건에 맞지 않으면 비활성화
        for (int i = activeUIInstances.Count - 1; i >= 0; i--)
        {
            InteractionUI uiInstance = activeUIInstances[i];
            Transform target = uiInstance.GetTarget();

            if (target == null)
            {
                // 타겟이 파괴되었거나 비활성화된 경우 UI 인스턴스 비활성화
                uiInstance.gameObject.SetActive(false);
                uiPool.Enqueue(uiInstance);
                activeUIInstances.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(player.position, target.position);

            if (distance < interactionDistance)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
                uiInstance.transform.position = screenPosition;
            }
            else
            {
                uiInstance.gameObject.SetActive(false);
                uiPool.Enqueue(uiInstance);
                activeUIInstances.RemoveAt(i);
            }
        }

        // 새로운 상호작용 오브젝트에 대해 UI를 활성화
        for (int i = 0; i < targetObjects.Count; i++)
        {
            Transform target = targetObjects[i];
            if (target == null || !target.gameObject.activeInHierarchy) continue;

            string key = interactionKeys[i];
            string description = descriptions[i];

            // 이미 UI가 활성화된 오브젝트는 스킵
            if (activeUIInstances.Exists(ui => ui.GetTarget() == target))
                continue;

            float distance = Vector3.Distance(player.position, target.position);

            if (distance < interactionDistance)
            {
                if (uiPool.Count > 0)
                {
                    InteractionUI uiInstance = uiPool.Dequeue();
                    if (Camera.main != null)
                    {
                        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.position);
                        uiInstance.transform.position = screenPosition;
                        uiInstance.SetKeyText(key, description); // 키와 설명 설정
                        uiInstance.SetTarget(target); // 타겟 설정
                        uiInstance.gameObject.SetActive(true);
                        activeUIInstances.Add(uiInstance);
                    }
                }
            }
        }
    }

    public void Init()
    {
        // canvas를 다시 찾음
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        uiPool = new Queue<InteractionUI>();
        player = GameManager.instance.playerTransfrom;
        for (int i = 0; i < maxUIElements; i++)
        {
            InteractionUI uiInstance = Instantiate(interactionUIPrefab, canvas.transform);
            uiInstance.gameObject.SetActive(false); // 처음에는 비활성화
            uiPool.Enqueue(uiInstance);
            uiInstance.transform.SetAsFirstSibling();
        }
    }

    // 새로운 상호작용 오브젝트와 키를 추가하는 메서드
    // 새로운 상호작용 오브젝트와 키를 추가하거나 기존 오브젝트의 설명을 변경하는 메서드
    public void AddInteractionObject(Transform newObject, string key, string description)
    {
        int index = targetObjects.IndexOf(newObject);

        if (index >= 0)
        {
            // 이미 존재하는 오브젝트의 키와 설명을 변경
            interactionKeys[index] = key;
            descriptions[index] = description;
        }
        else
        {
            // 새로운 오브젝트를 추가
            targetObjects.Add(newObject);
            interactionKeys.Add(key);
            descriptions.Add(description);
        }
    }


    // 특정 오브젝트의 UI를 끄는 메서드
    public void DisableUIForObject(Transform target)
    {
        int index = targetObjects.IndexOf(target);
        if (index >= 0)
        {
            // targetObjects, interactionKeys, descriptions에서 제거
            targetObjects.RemoveAt(index);
            interactionKeys.RemoveAt(index);
            descriptions.RemoveAt(index);
        }

        // 활성화된 UI 인스턴스 중 해당 타겟을 가진 인스턴스를 찾음
        InteractionUI uiInstance = activeUIInstances.Find(ui => ui.GetTarget() == target);
        if (uiInstance != null)
        {
            uiInstance.SetTarget(null); // 타겟 제거
            uiInstance.gameObject.SetActive(false);
            uiPool.Enqueue(uiInstance);
            activeUIInstances.Remove(uiInstance);
        }
    }
}
