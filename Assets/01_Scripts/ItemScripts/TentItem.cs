using System.Collections;
using UnityEngine;

public class TentItem : MonoBehaviour
{
    public GameObject priviewTent;
    public GameObject realTent;
    public GameObject tentTriggerColliders;
    public ItemData itemData;
    public Collider tentCollider;
    public Collider layingCollider;
    public Renderer tentPreviewRenderer;

    public GameObject backpack;

    public void TentColliderEnable(bool isOn)
    {
        tentCollider.enabled = isOn;
        layingCollider.enabled = isOn;
        priviewTent.SetActive(!isOn);
        realTent.SetActive(isOn);
        tentTriggerColliders.SetActive(isOn);
    }
}
