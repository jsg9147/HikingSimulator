using UnityEngine;
using TMPro;

public class DescriptionPopup : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text descriptionText;
    public TMP_Text priceText;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        TraceMousePos();
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
        Vector2 adjustedPosition = mousePosition;

        rectTransform.pivot = Vector2.up;

        // 팝업이 화면 오른쪽 밖으로 나가는 경우
        if (mousePosition.x + rectTransform.rect.width > Screen.width)
        {
            rectTransform.pivot = new(1, rectTransform.pivot.y);
        }

        // 팝업이 화면 왼쪽 밖으로 나가는 경우
        if (mousePosition.x < 0)
        {
            rectTransform.pivot = new Vector2(0, rectTransform.pivot.y);
        }

        // 팝업이 화면 아래쪽 밖으로 나가는 경우
        if (mousePosition.y - rectTransform.rect.height < 0)
        {
            rectTransform.pivot = new Vector2(rectTransform.pivot.x, 0);
        }

        // 팝업이 화면 위쪽 밖으로 나가는 경우
        if (mousePosition.y > Screen.height)
        {
            rectTransform.pivot = new(rectTransform.pivot.x, 1);
        }

        transform.SetAsLastSibling();
        rectTransform.position = adjustedPosition;
    }





    public void Show(ItemData itemData)
    {
        itemNameText.text = itemData.itemName;
        descriptionText.text = itemData.description;
        priceText.text = Mathf.FloorToInt(itemData.price * ShopManager.instance.resellPriceRate).ToString();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
