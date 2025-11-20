using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InventoryManager.instance.InitializeManagers();
        EquipmentManager.instance.RestoreEquippedItems();
        CombinationUIManager.instance.InitializeCombinationUI();
        GameManager.instance.tentPlacement.TakeDownTent();
        ShopManager.instance.ShopInit();
        UIManager.instance.UIInitialize();
        QuestManager.instance.CompanionCheck();
        if (CameraController.instance != null)
        {
            CameraController.instance.SetMainCamera();
        }
    }
}
