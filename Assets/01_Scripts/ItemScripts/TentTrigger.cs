using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentTrigger : MonoBehaviour
{
    public TentItem tentItem;

    private Transform tentCenter; // 텐트의 중앙 위치
    // private bool playerInTent = false;

    void Start()
    {
        tentCenter = GetComponent<Transform>(); // 텐트의 Transform을 중앙으로 사용
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStateController>().SetTentTransform(transform);
            print("Player entered the tent.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerStateController>().TentOut();
            print("Player exited the tent.");
        }
    }

    void MovePlayerToCenter(GameObject player)
    {
        player.transform.position = tentCenter.position;
        Debug.Log("Player moved to tent center.");
    }
}
