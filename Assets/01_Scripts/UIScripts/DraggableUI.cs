using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform draggableArea; // 드래그를 감지할 영역

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isDragging = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        // Load saved position
        LoadPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 클릭한 위치가 드래그 가능한 영역인지 확인
        if (RectTransformUtility.RectangleContainsScreenPoint(draggableArea, eventData.position, eventData.pressEventCamera))
        {
            isDragging = true;
            BringToFront();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 delta = eventData.delta / canvas.scaleFactor;
            rectTransform.anchoredPosition += delta;

            // Save position
            SavePosition();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void SavePosition()
    {
        PlayerPrefs.SetFloat($"{gameObject.name}_PosX", rectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat($"{gameObject.name}_PosY", rectTransform.anchoredPosition.y);
        PlayerPrefs.Save();
    }

    private void LoadPosition()
    {
        if (PlayerPrefs.HasKey($"{gameObject.name}_PosX") && PlayerPrefs.HasKey($"{gameObject.name}_PosY"))
        {
            float x = PlayerPrefs.GetFloat($"{gameObject.name}_PosX");
            float y = PlayerPrefs.GetFloat($"{gameObject.name}_PosY");
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    /// <summary>
    /// Brings the UI element to the front.
    /// </summary>
    private void BringToFront()
    {
        transform.SetAsLastSibling();
    }
}
