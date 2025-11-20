using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialougueUI;
    public TMP_Text dialogueText;
    public Image cursorImage; // 화살표 이미지
    private Queue<string> sentences = new Queue<string>();
    private bool isDisplaying = false;
    //private QuestGiver questGiver; // 대화의 주체가 되는 QuestGiver
    private Quest quest;

    public QuestGiver questGiver;

    private DeliveryTarget deliveryTarget;

    void Start()
    {
        sentences = new Queue<string>();
        cursorImage.gameObject.SetActive(false); // 시작할 때 커서 비활성화
    }

    void Update()
    {
        // 마우스 클릭을 감지하여 다음 문장으로 넘어가기
        if (isDisplaying && Input.GetMouseButtonDown(0))
        {
            DisplayNextSentence();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isDisplaying = false;
            dialogueText.text = string.Empty;
            sentences.Clear();
            dialougueUI.SetActive(false);
            gameObject.SetActive(false);
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
    }

    public void StartDialogue(Quest quest)
    {
        dialougueUI.SetActive(true);
        this.quest = quest;
        sentences.Clear();

        List<string> log = quest.dialogue;

        QuestStatus questStatus = QuestManager.instance.GetQuestStatus(quest);

        if (questStatus == QuestStatus.NotReceived)
        {
            log = quest.dialogue;
        }
        else if(questStatus == QuestStatus.Completed || questStatus == QuestStatus.Cleared)
        {
            log = quest.compliteDialogue;
        }

        foreach (string sentence in log)
        {
            sentences.Enqueue(sentence);
        }

        dialogueText.text = string.Empty;
        cursorImage.gameObject.SetActive(true);
        isDisplaying = true;

        DisplayNextSentence();
    }


    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = string.Empty;
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; // 한 프레임 대기
        }

        cursorImage.gameObject.SetActive(true); // 문장이 끝나면 커서 활성화
        StartCoroutine(BlinkCursor());
    }

    IEnumerator BlinkCursor()
    {
        while (isDisplaying)
        {
            cursorImage.enabled = true;
            yield return new WaitForSeconds(0.5f);
            cursorImage.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void EndDialogue()
    {
        dialougueUI.SetActive(false);
        cursorImage.gameObject.SetActive(false); // 대화가 끝나면 커서 비활성화
        isDisplaying = false;
        if (questGiver != null)
        {
            questGiver.MoveResume();
        }
        if (QuestManager.instance.GetQuestStatus(quest) == QuestStatus.Completed)
        {
            if (questGiver != null)
            {
                questGiver.CompleteQuest();
            }
            else if (deliveryTarget != null)
            {
                deliveryTarget.QuestClear();
            }
        }
        else if(QuestManager.instance.GetQuestStatus(quest) == QuestStatus.Cleared)
        {

        }
        else
        {
            if(!QuestManager.instance.QuestContains(questGiver.quest))
                UIManager.instance.ShowQuestAcceptUI(questGiver); // 퀘스트 수락/거절 창 표시
        }
        gameObject.SetActive(false);
    }

    public void CloseDialogueUI()
    {
        EndDialogue();
    }

    public void SetDeliveryTarget(DeliveryTarget target) => deliveryTarget = target;
    public void SetQuestGiver(QuestGiver questGiver) => this.questGiver = questGiver;
}
