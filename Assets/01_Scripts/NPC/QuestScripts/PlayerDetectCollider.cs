using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectCollider : MonoBehaviour
{
    private QuestGiver questGiver;

    private void Start()
    {
        questGiver = GetComponentInParent<QuestGiver>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed += InteractionPlayer;
            InteractionUIManager.instance.AddInteractionObject(transform, "E", "Talk");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed -= InteractionPlayer;
            InteractionUIManager.instance.DisableUIForObject(transform);
        }
    }

    private void InteractionPlayer()
    {
        questGiver.InteractionPlayer();
    }
}
