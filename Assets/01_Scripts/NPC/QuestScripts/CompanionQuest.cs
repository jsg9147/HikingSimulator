using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Companion Quest", menuName = "Quests/Companion Quest")]
public class CompanionQuest : Quest
{
    public string destinationName;

    public void ArriveAtTheDestination() => QuestItemUpdate(true);
}
