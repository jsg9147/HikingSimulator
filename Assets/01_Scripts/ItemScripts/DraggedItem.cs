// DraggedItemIcon.cs
using UnityEngine;
using UnityEngine.UI;

public class DraggedItem : MonoBehaviour
{
    public Image icon;
    public RectTransform rectTransform;

    private void OnEnable()
    {
        TraceMousePos();
    }
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            TraceMousePos();
        }
    }

    void TraceMousePos()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 adjustedPosition = mousePosition + new Vector2(rectTransform.rect.width / 2 + 0.1f, -rectTransform.rect.height / 2 + 0.1f);

        // 화면 밖으로 나가지 않게 조정
        if (adjustedPosition.x + rectTransform.rect.width > Screen.width)
        {
            adjustedPosition.x = mousePosition.x - rectTransform.rect.width / 2 - 0.1f;
        }
        if (adjustedPosition.y - rectTransform.rect.height < 0)
        {
            adjustedPosition.y = mousePosition.y + rectTransform.rect.height / 2 - 0.1f;
        }
        transform.SetAsLastSibling();
        rectTransform.position = adjustedPosition;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.sprite = sprite;
        icon.enabled = sprite != null;
    }
}
