using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetVendorTrigger : MonoBehaviour
{
    public RandomPathMover randomPath;
    private void Start()
    {
        randomPath.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            randomPath.gameObject.SetActive(true);
        }
    }
}
