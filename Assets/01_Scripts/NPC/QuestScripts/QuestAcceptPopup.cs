using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestAcceptPopup : MonoBehaviour
{
    public TMP_Text questNameText;
    public TMP_Text questDescriptionText;
    private QuestGiver questGiver;
    private Quest quest;

    public void ShowQuestDetails(Quest quest)
    {
        if (quest == null || quest == null)
        {
            Debug.LogWarning("QuestGiver or Quest is null!");
            return;
        }

        this.quest = quest;
        //if (questNameText != null) questNameText.text = quest.questName;
        if (questDescriptionText != null) questDescriptionText.text = quest.description;
        gameObject.SetActive(true);
    }

    public void SetQuestGiver(QuestGiver questGiver) => this.questGiver  = questGiver;

    public void AcceptQuest()
    {
        if (questGiver != null)
        {
            questGiver.GiveQuest();
        }
        if (UIManager.instance != null)
        {
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
        gameObject.SetActive(false);
    }

    public void DeclineQuest()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
        gameObject.SetActive(false);
    }

    public void AbandonQuest()
    {
        if (questGiver != null)
        {
            questGiver.AbandonQuest();
        }
        if (UIManager.instance != null)
        {
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
        gameObject.SetActive(false);
    }

    public void CloseQuestUI()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
        gameObject.SetActive(false);
    }
}

