using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelCompanion : MonoBehaviour
{
    private NPCFollow npcFollow;
    void Start()
    {
        npcFollow = GetComponent<NPCFollow>();
    }
}
