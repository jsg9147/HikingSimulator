using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    public List<ItemData> shopItems; // 상점의 아이템 목록
    public float interactionDistance = 3f; // 상호작용 거리
    public KeyCode interactionKey = KeyCode.E; // 상호작용 키

    private Transform playerTransform;
    private ShopManager shopManager; // 상점 관리자
    private Animator animator;

    RandomPathMover randomPathMover;
    void Start()
    {
        randomPathMover = GetComponent<RandomPathMover>();
        playerTransform = GameObject.FindWithTag("Player").transform; // 플레이어의 Transform 가져오기
        shopManager = ShopManager.instance;
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed += InterationPlayer;
            InteractionUIManager.instance.AddInteractionObject(transform, "E", "Talk");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed -= InterationPlayer;
            InteractionUIManager.instance.DisableUIForObject(transform);
        }
    }

    void InterationPlayer()
    {
        InteractWithShop();

        if (randomPathMover != null)
        {
            if (shopManager.shopUI.gameObject.activeSelf)
            {
                randomPathMover.StopMoving();
            }
            else
            {
                randomPathMover.ResumeMoving();
            }
        }
    }

    void InteractWithShop()
    {
        if (shopManager.shopUI.gameObject.activeSelf)
        {
            shopManager.CloseShop(); // 상점 UI가 열려있으면 닫기
            if(randomPathMover != null)
                randomPathMover.ResumeMoving();
        }
        else
        {
            shopManager.OpenShop(); // 상점 UI가 닫혀있으면 열기
            
            if (shopManager.shopItems != shopItems)
            {
                shopManager.SetShopItems(shopItems); // 상점 아이템 목록 설정
            }

            if (randomPathMover)
            {
                randomPathMover.StopMoving();
            }
        }
    }
}
