using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public Transform itemContainer;

    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        InventoryManager.instance.isSellMode = true;
        InventoryUIManager.instance.InventoryOn();
        GameManager.instance.playerMovement.SetMovementBlocked(true);
    }

    private void OnDisable()
    {
        if(InventoryManager.instance != null)
            InventoryManager.instance.isSellMode = false;
        if(UIManager.instance != null)  
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        InventoryUIManager.instance.InventoryUIOff();
        GameManager.instance.playerMovement.SetMovementBlocked(false);
    }
}
