using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 60f;  // 회전 속도를 조절하는 변수

    void Update()
    {
        // 매 프레임마다 Y축을 중심으로 회전
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
