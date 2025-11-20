using UnityEngine;

public class SelfCateringArea : MonoBehaviour
{
    public KeyCode activationKey = KeyCode.E; // 특정 키 (여기서는 E 키)
    private bool isPlayerNear = false; // 플레이어가 트리거 영역에 있는지 여부

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed += CombinationUIManager.instance.ToggleCombinationUI;
            InteractionUIManager.instance.AddInteractionObject(transform, "E", "Cook");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InputManager.instance.OnInteractKeyPressed -= CombinationUIManager.instance.ToggleCombinationUI;

            CombinationUIManager.instance.CombinationUIOff();
            InventoryUIManager.instance.InventoryUIOff();
            InteractionUIManager.instance.DisableUIForObject(transform);
        }
    }
}
