using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombinationUIManager : SingletonBase<CombinationUIManager>
{
    public List<CombinationSlot> combinationSlots; // 조합 슬롯 UI
    public CombinationUI combinationUIPrefab; // 조합 UI 패널

    private CombinationUI combinationUI; // 조합 UI 패널
    private CombinationManager combinationManager;
    private Inventory inventory;
    private bool isCombinationUIOpen = false; // 조합 UI 열림 상태 변수

    ItemData resultItem;
    public bool CombinationUIActiveSelf => isCombinationUIOpen;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        combinationManager = GetComponent<CombinationManager>();
        inventory = FindObjectOfType<Inventory>();
    }

    public void InitializeCombinationUI()
    {
        if (combinationUI == null)
        {
            combinationUI = Instantiate(combinationUIPrefab);
            combinationUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
            combinationUI.transform.SetAsFirstSibling();
            combinationUI.gameObject.SetActive(false);

            combinationUI.combinationBtn.onClick.AddListener(CombinationEvent);

            combinationSlots = combinationUI.slots;
        }
    }

    void CombinationEvent()
    {
        GetCombinationItem();
        CombinationUIOff();
        UIManager.instance.CursorChange(CursorLockMode.Locked);
        UIManager.instance.ProgressBar(3.5f);
        StartCoroutine(CombinationDelay(3.5f));
    }

    IEnumerator CombinationDelay(float time)
    {
        GameManager.instance.playerStateController.SetState(PlayerState.Cooking);
        yield return new WaitForSeconds(time);
        GameManager.instance.playerStateController.WorkEnd();
        InventoryManager.instance.AddItem(resultItem);

        resultItem = null;
    }

    public void ToggleCombinationUI()
    {
        if (GameManager.instance.playerStateController.CurrentState != PlayerState.Idle)
        {
            combinationUI.gameObject.SetActive(false);
            return;
        }

        isCombinationUIOpen = !isCombinationUIOpen;
        if(isCombinationUIOpen)
        {
            UIManager.instance.OpenUI(combinationUI.gameObject);
            InventoryUIManager.instance.InventoryUIOff();
            combinationUI.combinationBtn.interactable = resultItem != null;
        }
        else
        {
            combinationUI.gameObject.SetActive(false);
        }
        UIManager.instance.CursorChange(isCombinationUIOpen ? CursorLockMode.None : CursorLockMode.Locked);
    }

    public void CombinationUIOff()
    {
        isCombinationUIOpen = false;
        combinationUI.gameObject.SetActive(false);
    }

    public void AttemptCombination()
    {
        List<ItemData> itemsToCombine = new List<ItemData>();

        foreach (var slot in combinationSlots)
        {
            if (slot.GetItem() != null)
            {
                itemsToCombine.Add(slot.GetItem());
            }
        }

        resultItem = combinationManager.CombineItemsResult(itemsToCombine);

        combinationUI.combinationBtn.interactable = resultItem != null;
    }

    public void GetCombinationItem()
    {
        List<ItemData> itemsToCombine = new List<ItemData>();

        foreach (var slot in combinationSlots)
        {
            if (slot.GetItem() != null)
            {
                itemsToCombine.Add(slot.GetItem());
            }
            slot.ClearSlot();
        }
        
        combinationManager.DecreaseItemQuantities(itemsToCombine);
    }

    public void UpdateCombinationSlotsUI()
    {
        foreach (var slot in combinationSlots)
        {
            if (slot.GetItem() != null)
            {
                slot.icon.sprite = slot.GetItem().icon;
                slot.icon.enabled = true;
            }
            else
            {
                slot.icon.enabled = false;
            }
        }
    }

    public bool IsCombinationUIOpen()
    {
        return isCombinationUIOpen;
    }
}
