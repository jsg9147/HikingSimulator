using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPos : MonoBehaviour
{
    // Start is called before the first frame update

    bool startPos = false;
    void Start()
    {
        if (GameManager.instance != null && GameManager.instance.playerMovement != null)
        {
            GameManager.instance.playerTransfrom.position = transform.position;
            GameManager.instance.playerTransfrom.rotation = transform.rotation;
        }
    }

    private void Update()
    {
        if (!startPos)
        {
            GameManager.instance.playerTransfrom.position = transform.position;
            GameManager.instance.playerTransfrom.rotation = transform.rotation;
            GameManager.instance.playerMovement.Init();

            startPos = true;
        }
    }
}
