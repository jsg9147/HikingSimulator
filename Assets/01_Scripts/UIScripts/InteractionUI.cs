using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public TMP_Text text; // 텍스트를 표시할 TMP_Text 컴포넌트
    public TMP_Text descriptionText; 
    private Transform target; // 타겟 오브젝트

    // 키 텍스트를 설정하는 메서드
    public void SetKeyText(string key, string description = "")
    {
        text.text = key;
        descriptionText.text = description;
    }

    // 타겟 설정 메서드
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // 타겟을 반환하는 메서드
    public Transform GetTarget()
    {
        return target;
    }
}
