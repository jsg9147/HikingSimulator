using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestGiver : MonoBehaviour
{
    public Quest quest; // Quest 또는 DeliveryQuest 할당
    public bool isMotelNpc = false;
    public GameObject questionMark;
    public GameObject completeMark;

    RandomPathMover pathMover;

    bool isComplite;

    void Start()
    {
        pathMover = GetComponent<RandomPathMover>();
        questionMark.SetActive(true);
    }

    void Update()
    {
        if (quest != null)
        {
            completeMark.SetActive(QuestManager.instance.GetQuestStatus(quest) == QuestStatus.Completed);
            questionMark.SetActive(QuestManager.instance.GetQuestStatus(quest) == QuestStatus.NotReceived);
        }
    }
    bool QuestionMarkCheck()
    {
        bool notReceievd = QuestManager.instance.GetQuestStatus(quest) == QuestStatus.NotReceived;

        return false;
    }

    public void InteractionPlayer()
    {
        StartDialogue();
        if (pathMover != null)
        {
            pathMover.StopMoving();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMotelNpc)
        {
            if (other.CompareTag("Player"))
            {
                InputManager.instance.OnInteractKeyPressed += InteractionPlayer;
                InteractionUIManager.instance.AddInteractionObject(transform, "E", "Talk");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isMotelNpc)
        {
            if (other.CompareTag("Player"))
            {
                InputManager.instance.OnInteractKeyPressed -= InteractionPlayer;
                InteractionUIManager.instance.DisableUIForObject(transform);
            }
        }
    }

    void StartDialogue()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.SetQuestGiver(this);
            UIManager.instance.StartDialogue(quest);
        }
    }

    public void GiveQuest()
    {
        if (quest == null)
        {
            Debug.LogWarning("Quest is null!");
            return;
        }

        if (QuestManager.instance != null)
        {
            if (!QuestManager.instance.QuestContains(quest))
            {
                QuestManager.instance.AcceptQuest(quest);
            }
            Debug.Log("Quest given: " + quest.questName);

            // CompanionQuest일 경우, 동반자를 활성화
            if (quest is CompanionQuest companionQuest)
            {
                GetComponent<NPCFollow>().MoveStart();
                QuestManager.instance.SetCompanionNPCs(this);
            }
        }
        else
        {
            Debug.LogWarning("QuestManager instance is null!");
        }

        if (TownManager.instance != null)
        {
            TownManager.instance.QuestNPCUpdate();
        }
    }


    public void MoveResume()
    {
        if (pathMover != null)
        {
            pathMover.ResumeMoving();
        }
    }

    public void CompleteQuest()
    {
        if (quest.isCompleted)
            return;

        InventoryManager.instance.AddItem(quest.rewardItem);

        foreach (var item in quest.questItems.Keys)
        {
            InventoryManager.instance.RemoveItem(item, quest.questItems[item]);
        }
        quest.isCompleted = true;

        QuestManager.instance.CompliteQuest(quest);

        if (GetComponent<NPCFollow>() != null)
        {
            GetComponent<NPCFollow>().MoveStop();
        }
    }

    public void AbandonQuest()
    {
        if (!quest.isCompleted)
        {
            quest.isAbandoned = true;
            if (QuestManager.instance != null)
            {
                QuestManager.instance.QuestAbandon(quest);
            }
            Debug.Log("Quest abandoned: " + quest.questName);
        }
    }
}
