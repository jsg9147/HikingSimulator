using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    public static TownManager instance; 

    public List<QuestGiver> questGivers;
    public List<DeliveryTarget> deliveryTargets;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //GameManager.instance.playerMovement.shouldLimitRotation = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void QuestNPCUpdate()
    {
        foreach (var npc in deliveryTargets)
        {
            npc.SetTargetQuest();
        }
    }
}
