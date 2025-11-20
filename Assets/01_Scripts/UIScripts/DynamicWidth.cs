using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DynamicWidth : MonoBehaviour
{
    public RectTransform rectTransform;
    private TextMeshProUGUI textMeshProUGUI;

    void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();

        // rectTransform을 TextMeshProUGUI 컴포넌트가 붙어 있는 GameObject의 RectTransform으로 설정합니다.
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        // 텍스트에 따라 크기를 조정합니다.
        AdjustWidth();
    }

    void AdjustWidth()
    {
        // TextMeshProUGUI 컴포넌트의 텍스트 바운드를 가져옵니다.
        textMeshProUGUI.ForceMeshUpdate();
        var textBounds = textMeshProUGUI.textBounds;

        // RectTransform의 폭을 텍스트 바운드의 폭으로 설정합니다.
        rectTransform.sizeDelta = new Vector2(textBounds.size.x, rectTransform.sizeDelta.y);
    }
}
