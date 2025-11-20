using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestListUI : MonoBehaviour
{
    public QuestListItem questListItemPrefab; // 퀘스트 목록 항목 프리팹
    public Transform questListContainer; // 퀘스트 목록 항목들을 배치할 컨테이너

    void OnEnable()
    {
        UpdateQuestList();
    }

    public void UpdateQuestList()
    {
        if (questListItemPrefab == null || questListContainer == null)
        {
            Debug.LogWarning("QuestListItemPrefab or QuestListContainer is null!");
            return;
        }

        foreach (Transform child in questListContainer.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject);
        }

        if (QuestManager.instance != null && QuestManager.instance.CurrentQuests != null)
        {
            foreach (Quest quest in QuestManager.instance.CurrentQuests)
            {
                QuestListItem questListItem = Instantiate(questListItemPrefab, questListContainer);
                questListItem.SetQuestInfo(quest);
            }
        }
        else
        {
            Debug.LogWarning("GameManager instance is null!");
        }
    }
}
