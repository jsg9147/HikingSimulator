using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager instance;

    [Header("UI Prefabs")]
    public InventoryUI inventoryUIPrefab; // ÀÎº¥Åä¸® UI ÇÁ¸®ÆÕ
    public GameObject popupPrefab; // ÆË¾÷ UI ÇÁ¸®ÆÕ
    public DescriptionPopup descriptionPopupPrefab; // ÆË¾÷ UI ÇÁ¸®ÆÕ

    [Header("UI Elements")]
    private InventoryUI inventoryUI; // ÀÎº¥Åä¸® UI
    public GameObject popup; // ÆË¾÷ UI
    private DescriptionPopup descriptionPopup;

    public InventoryUI InventoryUI => inventoryUI;

    private Dictionary<string, int> inventoryPosInfo;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        InputManager.instance.OnInventoryKeyPressed += ToggleInventoryUI;
    }

    private void OnDisable()
    {
        if(InputManager.instance != null)
            InputManager.instance.OnInventoryKeyPressed -= ToggleInventoryUI;
    }

    private void OnDestroy()
    {
        if (InputManager.instance != null)
            InputManager.instance.OnInventoryKeyPressed -= ToggleInventoryUI;
    }

    public void InitializeInventoryUI()
    {
        inventoryUI = Instantiate(inventoryUIPrefab);
        inventoryUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
        inventoryUI.SetItemPosInfo(inventoryPosInfo);

        InventoryManager.instance.SetInventorySlots(inventoryUI.InventorySlots());
        inventoryUI.gameObject.SetActive(false);

        popup = Instantiate(popupPrefab);
        popup.transform.SetParent(GameObject.Find("Canvas").transform, false);
        popup.SetActive(false);

        descriptionPopup = Instantiate(descriptionPopupPrefab);
        descriptionPopup.transform.SetParent(GameObject.Find("Canvas").transform, false);
        descriptionPopup.gameObject.SetActive(false);
        descriptionPopup.transform.SetAsLastSibling();
    }
    public void SetInventoryItemPos(Dictionary<string, int> itemPosInfo)
    {
        this.inventoryPosInfo = itemPosInfo;
    }

    public void ToggleInventoryUI()
    {
        bool isActive = !inventoryUI.gameObject.activeSelf;
        if (!CombinationUIManager.instance.CombinationUIActiveSelf)
        {
            if (isActive)
            {
                UIManager.instance.OpenUI(inventoryUI.gameObject);
            }
            else
            {
                inventoryUI.gameObject.SetActive(isActive);
                UIManager.instance.CursorChange(CursorLockMode.Locked);
                InventoryUI.SaveItemIndex();
            }
        }
        //ChangeInventoryPopupPosition(Vector2.zero);
        if (!isActive)
        {
            DescriptionPopupOff();
        }
    }

    public void InventoryOn()
    {
        if (!inventoryUI.gameObject.activeSelf)
        {
            ToggleInventoryUI();
        }
    }

    public void InventoryUIOff()
    {
        inventoryUI.gameObject.SetActive(false);
        InventoryUI.SaveItemIndex();
        DescriptionPopupOff();
    }

    public void UpdateInventoryUI()
    {
        inventoryUI.SyncSlotsWithInventory();
    }

    public void SetInventorySlots(int backpackSize)
    {
        inventoryUI.SlotUpdate(backpackSize);
    }

    public void ShowPopup(Vector3 position, UnityAction useAction, UnityAction removeAction)
    {
        popup.SetActive(true);
        popup.transform.position = position;

        Button useButton = popup.transform.Find("UseButton").GetComponent<Button>();
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(useAction);

        Button removeButton = popup.transform.Find("RemoveButton").GetComponent<Button>();
        removeButton.onClick.RemoveAllListeners();
        removeButton.onClick.AddListener(removeAction);
    }

    public void DescriptionPopupOn(ItemData item)
    {
        if (item == null)
            return;

        descriptionPopup.gameObject.SetActive(true);
        descriptionPopup.Show(item);
    }

    public void DescriptionPopupOff()
    {
        if (descriptionPopup != null)
        {
            descriptionPopup.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Description popup null.");
        }
    }

    public void HidePopup()
    {
        popup.SetActive(false);
    }
}
