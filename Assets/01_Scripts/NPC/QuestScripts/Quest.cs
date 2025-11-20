using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea] public string description;
    public bool isCompleted;
    public bool isAbandoned; // 퀘스트 포기 상태 추가
    public bool orQuest;

    public List<string> dialogue;
    public List<string> compliteDialogue;
    public List<QuestItemPair> questItemsList; // 인스펙터에서 관리할 리스트
    public ItemData rewardItem;

    public Dictionary<ItemData, int> questItems; // 런타임에 사용할 딕셔너리

    public bool IsQuestFulfilled { get; private set; } // 퀘스트 조건 충족 여부 변수 추가

    public string QuestName => questName;
    public string Description => description;
    public bool IsCompleted => isCompleted;
    public List<string> Dialogue => dialogue;
    public List<string> CompliteDialogue => compliteDialogue;

    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        isCompleted = false; 
        isAbandoned = false;
        questItems = new Dictionary<ItemData, int>();
        foreach (var pair in questItemsList)
        {
            questItems[pair.ItemData] = pair.value;
        }
    }

    public bool CheckComplite()
    {
        return isCompleted;
    }

    public void QuestItemUpdate(Dictionary<ItemData, int> playerItems)
    {
        if (orQuest)
        {
            IsQuestFulfilled = OrCheckItemsSufficient(playerItems);
        }
        else
        {
            IsQuestFulfilled = CheckItemsSufficient(playerItems);
        }
    }

    public void QuestItemUpdate(bool isFulfilled)
    {
        IsQuestFulfilled = isFulfilled;
    }

    private bool CheckItemsSufficient(Dictionary<ItemData, int> playerItems)
    {
        if(questItems.Count == 0)
            return false;

        foreach (var questItem in questItems)
        {
            ItemData itemData = questItem.Key;
            int requiredCount = questItem.Value;

            if (!playerItems.ContainsKey(itemData) || playerItems[itemData] < requiredCount)
            {
                return false;
            }
        }
        return true;
    }

    private bool OrCheckItemsSufficient(Dictionary<ItemData, int> playerItems)
    {
        if (questItems.Count == 0)
            return false;

        foreach (var questItem in questItems)
        {
            ItemData itemData = questItem.Key;
            int requiredCount = questItem.Value;

            if (playerItems.ContainsKey(itemData) && playerItems[itemData] >= requiredCount)
            {
                return true;
            }
        }
        return false;
    }
}
