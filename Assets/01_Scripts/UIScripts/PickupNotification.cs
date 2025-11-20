using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickupNotification : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text countText;
    private float displayDuration = 2.0f; // 알림 표시 시간
    private float fadeDuration = 0.5f; // 페이드 아웃 시간

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetItemInfo(ItemData itemData, int itemCount = 1)
    {
        icon.sprite = itemData.icon;
        nameText.text = itemData.itemName;
        countText.text = "× " + itemCount.ToString();
    }

    public void ShowNotification()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(displayDuration);

        CanvasGroup canvasGroup = gameObject.AddComponent<CanvasGroup>();
        float elapsedTime = 0f;

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, startPos.y + 50); // 알림이 위로 이동

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;

        UIManager.instance.OnNotificationRemoved(this);

        Destroy(gameObject);
    }

    // 알림의 총 지속 시간을 반환
    public float GetTotalDuration()
    {
        return displayDuration + fadeDuration;
    }
}
