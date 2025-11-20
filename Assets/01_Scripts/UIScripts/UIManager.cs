using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : SingletonBase<UIManager>
{
    [Header("UI Prefabs")]
    public PausePopup pausePopupPrefab;
    public OptionsMenu optionMenuPrefab;
    public GameObject keyInfoPrefab;

    public MenualPopup menualPopupPrefab;
    //public QuestUI questUIPopupPrefab;

    public SurvivalStatUI survivalStatUIPrefab;
    public QuestAcceptPopup questAcceptUIPrefab;
    public QuestListUI QuestListUIPrefab;
    public DialogueUI dialogueUIPrefab;
    public RecipeBookManager recipebookManagerPrefab;
    public CurrencyUI currencyUIPrefab;
    public Image moveIconPrefab;
    public Image fadeImagePrefab;

    public ProgressBarPro progressBarProPrefab;

    [Header("UI Elements")]
    public PickupNotification pickupNotificationPrefab;

    public float fadeDuration = 3.0f; // 암전 및 회복 시간

    private Stack<GameObject> uiStack = new Stack<GameObject>();
    private Transform mainCanvas;

    private QuestListUI questListUI;
    private QuestAcceptPopup questAcceptPopup;

    //private QuestUI questUI;
    private MenualPopup menualPopup;

    private SurvivalStatUI survivalStatUI;
    private DialogueUI dialogueUI;
    private RecipeBookManager recipebookManager;

    private Queue<PickupNotification> notificationQueue = new Queue<PickupNotification>();
    private List<PickupNotification> activeNotifications = new List<PickupNotification>();
    private Image moveIcon;
    private Image fadeImage;

    private CurrencyUI currencyUI;
    private ProgressBarPro progressBarPro;

    private OptionsMenu optionsMenu;
    private PausePopup pausePopup;

    private void Start()
    {
        CursorChange(CursorLockMode.Locked);
        //UIInitialize();
    }

    private void OnEnable()
    {
        if (InputManager.instance != null)
        {
            InputManager.instance.OnQuestPopupPressed += MenualPopupToggle;
            InputManager.instance.OnEscKeyPressed += PausePopupOn;
        }
    }

    private void OnDisable()
    {
        if (InputManager.instance != null)
        {
            InputManager.instance.OnQuestPopupPressed -= MenualPopupToggle;
            InputManager.instance.OnEscKeyPressed -= PausePopupOn;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseTopUI();
        }
    }

    public void OpenUI(GameObject uiPanel)
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
            uiPanel.transform.SetAsLastSibling();
            uiStack.Push(uiPanel);
            CursorChange(CursorLockMode.None);
        }
    }

    private void CloseTopUI()
    {
        if (uiStack.Count > 0)
        {
            GameObject topUI = uiStack.Pop();
            if (topUI.gameObject.activeSelf)
            {
                topUI.SetActive(false);
            }
            else
            {
                CloseTopUI();
            }
            CursorChange(CursorLockMode.Locked);
        }
    }

    public void UIInitialize()
    {
        mainCanvas = GameObject.Find("Canvas").transform;

        questListUI = Instantiate(QuestListUIPrefab, mainCanvas, false);
        questListUI.transform.SetAsFirstSibling();
        questListUI.UpdateQuestList();

        questAcceptPopup = Instantiate(questAcceptUIPrefab, mainCanvas, false);
        menualPopup = Instantiate(menualPopupPrefab, mainCanvas, false);
        menualPopup.PopupOff();

        survivalStatUI = Instantiate(survivalStatUIPrefab, mainCanvas, false);
        survivalStatUI.gameObject.SetActive(true);

        dialogueUI = Instantiate(dialogueUIPrefab, mainCanvas, false);
        dialogueUI.gameObject.SetActive(false);

        recipebookManager = Instantiate(recipebookManagerPrefab, mainCanvas, false);
        recipebookManager.gameObject.SetActive(false);

        moveIcon = Instantiate(moveIconPrefab, mainCanvas, false);
        moveIcon.gameObject.SetActive(false);

        fadeImage = Instantiate(fadeImagePrefab, mainCanvas, false);
        fadeImage.gameObject.SetActive(false);

        currencyUI = Instantiate(currencyUIPrefab,mainCanvas, false);
        currencyUI.SetCurrency(InventoryManager.instance.currencyManager.GetCurrency());

        progressBarPro = Instantiate(progressBarProPrefab, mainCanvas, false);
        progressBarPro.gameObject.SetActive(false);

        InventoryManager.instance.UpdateCurrencyData();

        if (CombinationManager.instance != null)
        {
            CombinationManager.instance.SetReceipeBookUI(recipebookManager);
        }

        GameObject keyInfo = Instantiate(keyInfoPrefab, mainCanvas, false);

        optionsMenu = Instantiate(optionMenuPrefab, mainCanvas, false);
        pausePopup = Instantiate(pausePopupPrefab, mainCanvas, false);
        pausePopup.transform.SetAsLastSibling();
        optionsMenu.transform.SetAsLastSibling();

        pausePopup.optionButton.onClick.AddListener(optionsMenu.OptionPopupOn);
    }

    private void MenualPopupToggle()
    {
        if (menualPopup != null)
        {
            menualPopup.PopupToggle();
        }
    }

    public void QuestBoardUpdate()
    {
        if (questListUI != null)
        {
            questListUI.UpdateQuestList();
            //questUI.SetQuestList();
        }
    }

    public void CursorChange(CursorLockMode cursorMode)
    {
        Cursor.lockState = cursorMode;
        Cursor.visible = (cursorMode == CursorLockMode.None);
    }

    public void UpdateSurvivalStats(SurvivalStats survivalStats)
    {
        survivalStatUI.UpdateSurvivalStats(survivalStats);
    }

    public void StopInjuryEffect()
    {
        survivalStatUI.StopBlinkingInjuryIcon();
    }

    public void UpdateCurrencyUI(int currency)
    {
        if (currencyUI != null)
        {
            currencyUI.SetCurrency(currency);
        }
    }

    public void StartDialogue(Quest quest)
    {
        if (dialogueUI != null && !dialogueUI.gameObject.activeSelf)
        {
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.StartDialogue(quest);
        }
    }

    public void StartCompliteDialogue(Quest quest)
    {
        if (dialogueUI != null)
        {
            dialogueUI.gameObject.SetActive(true);
            dialogueUI.StartDialogue(quest);
        }
    }

    public void CloseDialogue()
    {
        if (dialogueUI != null)
        {
            dialogueUI.CloseDialogueUI();
            CursorChange(CursorLockMode.Locked);
        }
    }

    public void ShowQuestAcceptUI(QuestGiver questGiver)
    {
        if (questAcceptPopup != null && questGiver != null)
        {
            questAcceptPopup.SetQuestGiver(questGiver);
            questAcceptPopup.ShowQuestDetails(questGiver.quest);
            CursorChange(CursorLockMode.None);
        }
    }

    public void SetDeliveryTarget(DeliveryTarget target)
    {
        dialogueUI.SetDeliveryTarget(target);
    }

    public void SetQuestGiver(QuestGiver questGiver)
    {
        dialogueUI.SetQuestGiver(questGiver);
    }

    public void ShowPickupNotification(ItemData itemData, int itemCount = 1)
    {
        // 새로운 알림 인스턴스 생성
        PickupNotification newNotification = Instantiate(pickupNotificationPrefab, mainCanvas);

        // 알림의 정보 설정
        newNotification.SetItemInfo(itemData, itemCount);

        // 알림 위치 조정 (기존 알림 위에 새 알림을 표시)
        RectTransform rectTransform = newNotification.GetComponent<RectTransform>();
        float yOffset = activeNotifications.Count * 50f; // 각 알림 간의 간격
        rectTransform.anchoredPosition += new Vector2(0, yOffset);

        // 알림을 표시하고 리스트에 추가
        newNotification.ShowNotification();
        activeNotifications.Add(newNotification);
    }

    public void OnNotificationRemoved(PickupNotification notification)
    {
        if (activeNotifications.Contains(notification))
        {
            int removedIndex = activeNotifications.IndexOf(notification);
            activeNotifications.Remove(notification);

            // 이후 알림들을 한 칸씩 위로 올립니다.
            for (int i = removedIndex; i < activeNotifications.Count; i++)
            {
                RectTransform rectTransform = activeNotifications[i].GetComponent<RectTransform>();
                rectTransform.anchoredPosition -= new Vector2(0, 50f);
            }
        }
    }
    public Image MoveIcon(Sprite icon)
    {
        moveIcon.transform.SetAsLastSibling();
        moveIcon.sprite = icon;
        moveIcon.gameObject.SetActive(true);
        return moveIcon;
    }

    public Image MoveIcon()
    {
        return moveIcon;
    }

    public void FadeOutOnly()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOutOnlyCoroutine());
    }

    // 화면을 서서히 어둡게 하는 코루틴
    IEnumerator FadeOutOnlyCoroutine()
    {
        float elapsedTime = 0;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;

        SceneManager.LoadScene(0);
        SingletonManager.instance.ResetAllSingletons();
    }

    public void ProgressBar(float time)
    {
        progressBarPro.gameObject.SetActive(true);
        StartCoroutine(ProgressBarCoroutine(time));
    }

    IEnumerator ProgressBarCoroutine(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / time);
            progressBarPro.SetValue(progress, 1f);
            yield return null;
        }
        progressBarPro.gameObject.SetActive(false);
    }

    public void FadeOutAndIn()
    {
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(FadeOutAndInCoroutine());
    }

    // 화면을 서서히 어둡게 만든 후 서서히 밝게 만드는 코루틴
    IEnumerator FadeOutAndInCoroutine()
    {
        // Fade Out
        float elapsedTime = 0;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;
        // Fade In
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            if (GameManager.instance.playerStateController.CurrentState == PlayerState.Laying)
            {
                GameManager.instance.playerStateController.WakeUp();
            }
            elapsedTime += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(false);
    }

    public void QuestUpdate()
    {
        questListUI.UpdateQuestList();
    }

    void PausePopupOn()
    {
        if (uiStack.Count > 0)
        {
            // 스택에 쌓인 UI가 있다면 해당 UI를 끄고 PausePopupOn 호출을 막음
            CloseTopUI();
        }
        else if (pausePopup != null)
        {
            if(!optionsMenu.panel.gameObject.activeSelf)
                pausePopup.PausePopupOn();
            else
            {
                optionsMenu.OptionPopupOff();
            }
        }
    }
}
