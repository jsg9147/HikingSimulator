using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Delivery Quest", menuName = "Quests/Delivery Quest")]
public class DeliveryQuest : Quest
{
    public ItemData itemToDeliver;  // 배달할 아이템
    public string deliveryTargetName; // 배달할 대상 NPC의 이름

    public ItemData ItemToDeliver => itemToDeliver;
    public string DeliveryTargetName => deliveryTargetName;

}
