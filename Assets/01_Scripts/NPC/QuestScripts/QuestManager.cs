using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : SingletonBase<QuestManager>
{
    private List<Quest> currentQuests;
    private List<Quest> abandonQuests;
    private List<Quest> compliteQuest;

    private List<DeliveryQuest> deliveryQuests;
    private List<CompanionQuest> companionQuests;

    private List<QuestGiver> companionNPCs;


    private void Start()
    {
        currentQuests = new List<Quest>();
        companionQuests = new();
        abandonQuests = new();
        deliveryQuests = new();
        companionNPCs = new();
        compliteQuest = new();
    }

    public List<Quest> CurrentQuests { get { return currentQuests; } }

    public void AcceptQuest(Quest quest)
    {
        currentQuests.Add(quest);
        QuestItemUpdate();

        if (quest is DeliveryQuest)
        {
            deliveryQuests.Add((DeliveryQuest)quest);
        }

        if (quest is CompanionQuest)
        {
            companionQuests.Add((CompanionQuest)quest);
        }

        UIManager.instance.QuestBoardUpdate();
    }

    public void CompliteQuest(Quest quest)
    {
        compliteQuest.Add(quest);
        currentQuests.Remove(quest);

        UIManager.instance.QuestBoardUpdate();
    }

    public void SetCompanionNPCs(QuestGiver npc)
    {
        if (companionNPCs != null)
        {
            companionNPCs.Add(npc);
        }
    }

    public void QuestAbandon(Quest quest)
    {
        currentQuests.Remove(quest);
        abandonQuests.Add(quest);
        QuestItemUpdate();
        UIManager.instance.QuestBoardUpdate();
    }

    public void QuestItemUpdate()
    {
        if (currentQuests == null)
            return;

        foreach (Quest quest in currentQuests)
        {
            DeliveryQuest deliveryQuestCheck = quest as DeliveryQuest;
            if (deliveryQuestCheck == null)
            {
                Dictionary<ItemData, int> keyValuePairs = new Dictionary<ItemData, int>();
                foreach (var item in quest.questItems.Keys)
                {
                    keyValuePairs.Add(item, InventoryManager.instance.GetItemCount(item));
                }
                quest.QuestItemUpdate(keyValuePairs);
            }
        }
    }

    public bool QuestContains(Quest quest)
    {
        return currentQuests.Contains(quest);
    }

    public DeliveryQuest FindDeliveryQuest(string targetName)
    {
        DeliveryQuest targetQuest = deliveryQuests.Find(x => x.deliveryTargetName == targetName);
        return targetQuest;
    }

    public QuestStatus GetQuestStatus(Quest quest)
    {
        if (!currentQuests.Contains(quest) && !abandonQuests.Contains(quest) && !compliteQuest.Contains(quest))
        {
            return QuestStatus.NotReceived; // 퀘스트를 받지 않은 상태
        }
        else if (abandonQuests.Contains(quest))
        {
            return QuestStatus.Abandoned; // 퀘스트를 거절한 상태
        }
        else if (currentQuests.Contains(quest))
        {
            if (quest.IsCompleted)
            {
                return QuestStatus.Cleared; // 퀘스트를 클리어한 상태
            }
            else if (quest.IsQuestFulfilled)
            {
                return QuestStatus.Completed; // 퀘스트 조건이 완료된 상태
            }
            else
            {
                return QuestStatus.Received; // 퀘스트를 받은 상태
            }
        }
        else if (compliteQuest.Contains(quest))
            return QuestStatus.Cleared;

        return QuestStatus.Nothing; // 기본적으로는 아무 상태도 아닌 경우
    }
    public void CompanionCheck()
    {
        if (companionNPCs == null)
            return;

        foreach(QuestGiver giver in companionNPCs)
        {
            if (giver.quest is CompanionQuest)
            {
                CompanionQuest quest = (CompanionQuest)giver.quest;
                if (quest.destinationName == SceneManager.GetActiveScene().name)
                {
                    quest.ArriveAtTheDestination();
                }

                if (quest.isCompleted)
                {
                    Destroy(giver.gameObject);
                }
            }
        }
    }

    public void CompanionPositionUpdate()
    {
        if (companionNPCs == null)
            return;

        Vector3 companionPos = Vector3.right *2f;
        int index = 0;
        foreach (QuestGiver giver in companionNPCs)
        {
            giver.transform.position = Vector3.zero + companionPos * index;
            index++;
        }
    }
}

public enum QuestStatus
{
    NotReceived, // 퀘스트를 받지 않은 상태
    Received,    // 퀘스트를 받은 상태
    Completed,   // 퀘스트 조건이 완료된 상태
    Cleared,     // 퀘스트를 클리어한 상태
    Abandoned,    // 퀘스트를 거절한 상태
    Nothing
}
