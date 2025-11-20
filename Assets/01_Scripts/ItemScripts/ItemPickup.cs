using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    public List<ItemData> items; // 아이템 정보

    private Rigidbody rb;
    private int itemCount;
    private float throwForce = 5f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (InputManager.instance != null)
                InputManager.instance.OnPickUpKeyPressed += Pickup;
            InteractionUIManager.instance.AddInteractionObject(transform, "F", "Pick up");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (InputManager.instance != null)
                InputManager.instance.OnPickUpKeyPressed -= Pickup;
            InteractionUIManager.instance.DisableUIForObject(transform);
        }
    }

    private void OnDisable()
    {
        if(InputManager.instance != null)
            InputManager.instance.OnPickUpKeyPressed -= Pickup;
        if(InteractionUIManager.instance != null)
            InteractionUIManager.instance.DisableUIForObject(transform);
    }

    private void OnDestroy()
    {
        if (InputManager.instance != null)
            InputManager.instance.OnPickUpKeyPressed -= Pickup;
        if(InteractionUIManager.instance != null)
            InteractionUIManager.instance.DisableUIForObject(transform);
    }

    void Pickup()
    {
        if (items == null)
            return;

        GameManager.instance.playerStateController.PickUp();
        for (int i = 0; i< items.Count; i++)
        {
            Debug.Log("Picking up item: " + items[i].itemName);
            bool wasPickedUp = InventoryManager.instance.inventory.AddItem(items[i], 1);
            if (wasPickedUp)
            {
                InventoryUIManager.instance.UpdateInventoryUI();
                InteractionUIManager.instance.DisableUIForObject(transform); // UI 비활성화
            }
        }
        Destroy(gameObject);
    }

    public void SetItem(ItemData itemData, int count = 1)
    {
        for (int i = 0; i < count; i++) {
            items.Add(itemData);
        }
    }

    public void Throw()
    {
        if (items == null)
        {
            return;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        
        // 플레이어가 보는 방향으로 힘을 가한다
        Transform playerTransform = GameManager.instance.playerTransfrom; // 플레이어의 Transform 가져오기
        Vector3 throwDirection = playerTransform.forward; // 플레이어가 쳐다보는 방향

        rb.isKinematic = false; // 물리 연산이 가능하도록 설정
        rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange); // 던지는 힘 가하기
    }
}