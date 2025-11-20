using System.Collections.Generic;
using UnityEngine;

public class DeliveryTarget : MonoBehaviour
{
    public string targetName; // 배달 대상 NPC의 이름
    public bool playerInRange = false;
    public GameObject completeMark;
    DeliveryQuest targetQuest;

    private void Awake()
    {
        completeMark.SetActive(false);
    }

    private void Start()
    {
        SetTargetQuest();
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E) && targetQuest != null)
            {
                UIManager.instance.SetDeliveryTarget(this);
                if (completeMark.activeSelf)
                {
                    targetQuest.isCompleted = true;
                    UIManager.instance.StartDialogue(targetQuest);
                }
            }
        }
        if (targetQuest != null)
        {
            targetQuest.QuestItemUpdate(InventoryManager.instance.SearchItem(targetQuest.itemToDeliver));
            completeMark.SetActive(targetQuest.IsQuestFulfilled);
        }
    }
    void StartDialogue()
    {
        if (UIManager.instance != null)
        {   
            UIManager.instance.StartDialogue(targetQuest);
        }
    }

    public void SetTargetQuest()
    {
        targetQuest = QuestManager.instance.FindDeliveryQuest(targetName);
    }

    public void QuestClear()
    {
        if (InventoryManager.instance.SearchItem(targetQuest.itemToDeliver))
        {
            InventoryManager.instance.RemoveItem(targetQuest.itemToDeliver);
            InventoryManager.instance.AddItem(targetQuest.rewardItem);
            InventoryUIManager.instance.UpdateInventoryUI();
            targetQuest.isCompleted = true;
        }

        UIManager.instance.QuestBoardUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
