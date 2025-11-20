using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform questContain;
    public QuestListItem itemPrefab;

    private List<Quest> currentQuest;

    public void SetQuestList()
    {
        if (GameManager.instance == null)
        {
            return;
        }

        List<Quest> list = QuestManager.instance.CurrentQuests;

        if (currentQuest == list)
        {
            return;
        }

        currentQuest = new List<Quest>(list);
        // 기존의 모든 자식 오브젝트 삭제
        foreach (Transform child in questContain)
        {
            Destroy(child.gameObject);
        }

        // 새로운 퀘스트 리스트로 갱신
        foreach (Quest quest in currentQuest)
        {
            QuestListItem questItem = Instantiate(itemPrefab, questContain);
            questItem.SetQuestInfo(quest);
        }
    }
}
