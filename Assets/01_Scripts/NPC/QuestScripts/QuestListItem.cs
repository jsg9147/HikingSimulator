using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestListItem : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public GameObject compliteImage;

    private Quest quest;
    public void SetQuestInfo(Quest quest)
    {
        this.quest = quest;
        nameText.text = quest.QuestName;
        descriptionText.text = quest.Description;

        if (compliteImage != null)
        {
            compliteImage.SetActive(quest.isCompleted);
        }
    }

    public void RemoveQuest()
    {
        if (quest != null)
        {
            QuestManager.instance.QuestAbandon(quest);
        }
    }
}
