using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    private Transform playerTransform;
    private void FixedUpdate()
    {
        transform.position = playerTransform.position + (Vector3.up * 15f);
    }

    public void SetPlayer(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }
}
