using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Start()
    {
        transform.SetParent(GameCanavsManager.instance.transform);
    }
    void LateUpdate()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}
