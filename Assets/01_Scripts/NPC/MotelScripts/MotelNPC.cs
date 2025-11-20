using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotelNPC : MonoBehaviour
{
    public List<ItemData> shopItems; // 상점 아이템 목록
    //public QuestGiver questGiver;
    public MotelPopup motelPopup; // 휴식 기능 아이콘
    
    private ShopManager shopManager;

    void Start()
    {
        shopManager = ShopManager.instance;
        if (motelPopup != null)
        {
            motelPopup.gameObject.SetActive(false);
            //motelPopup.questButton.onClick.AddListener(QuestPopupOn);
            motelPopup.closeButton.onClick.AddListener(() =>
            {
                UIManager.instance.CursorChange(CursorLockMode.Locked);
                motelPopup.gameObject.SetActive(false);
            });

            motelPopup.buyButton.onClick.AddListener(() =>
            {
                shopManager.OpenShop();
                if (shopManager.shopItems != shopItems)
                {
                    shopManager.SetShopItems(shopItems); // 상점 아이템 목록 설정
                }
                motelPopup.gameObject.SetActive(false);
            });

            motelPopup.restButton.onClick.AddListener(() =>
            {
                //GetComponent<ScreenFade>().FadeOutAndIn();
                GameManager.instance.PlayerRecovery();
                motelPopup.gameObject.SetActive(false);
                UIManager.instance.CursorChange(CursorLockMode.Locked);
            });

            motelPopup.questButton.onClick.AddListener(() =>
            {
                //QuestPopupOn();
                motelPopup.gameObject.SetActive(false);
            });
        }
    }

    //void Update()
    //{
    //    if (questGiver != null)
    //    {
    //        questGiver.isMotelNpc = true; // Prevent QuestGiver from reacting to key press
    //    }
    //}

    void InteractionPlayer()
    {
        motelPopup.gameObject.SetActive(true);
        UIManager.instance.CursorChange(CursorLockMode.None);
    }

    //void QuestPopupOn()
    //{
    //    if (questGiver != null)
    //    {
    //        UIManager.instance.ShowQuestAcceptUI(questGiver);
    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed += InteractionPlayer;
            InteractionUIManager.instance.AddInteractionObject(transform, "E", "Talk");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 플레이어가 범위 내에서 벗어났을 때
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed -= InteractionPlayer;
            InteractionUIManager.instance.DisableUIForObject(transform);
        }
    }
}
