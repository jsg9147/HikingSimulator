using UnityEngine;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using System.Collections;

[System.Serializable]
public class ConsumableManager
{
    private SurvivalStats survivalStats;
    private InjuryManager injuryManager;

    public ConsumableManager(SurvivalStats survivalStats, InjuryManager injuryManager)
    {
        this.survivalStats = survivalStats;
        this.injuryManager = injuryManager;
    }

    // 소모품 아이템 사용 메서드
    public void UseConsumable(ItemData item)
    {
        if (item.healthRestore > 0)
        {
            GameManager.instance.playerStateController.Drink();
        }
        else
        {
            GameManager.instance.playerStateController.Eat();
        }
        GameManager.instance.survivalStatsManager.Recovery(item);

        if (item.healsMinorInjury || item.healsMajorInjury)
        {
            if (item.healsMinorInjury)
            {
                injuryManager.HealInjury(InjurySeverity.Minor);
            }

            if (item.healsMajorInjury)
            {
                injuryManager.HealInjury(InjurySeverity.Major);
                survivalStats.hunger *= 0.5f;
            }
            MasterAudio.PlaySound3DAtTransform("Kit", GameManager.instance.playerTransfrom);
        }
        

        Debug.Log("Used consumable item: " + item.itemName);
    }
}
