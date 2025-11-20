using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SurvivalStatUI : MonoBehaviour
{
    public Image healthBar;
    public Image hungerBar;

    public Image injuryIcon;

    public Color normalColor = Color.white;
    public Color warningColor = Color.red;
    public float warningThreshold = 0.3f;

    public float blinkInterval = 0.5f; // 색상이 전환되는 시간 간격
    private bool isBlinking = false;   // 깜박임이 활성화 되었는지 여부
    private Coroutine currentBlinkCoroutine; // 현재 실행 중인 깜박임 코루틴 참조

    public void UpdateSurvivalStats(SurvivalStats survivalStats)
    {
        UpdateStatUI(healthBar, survivalStats.health / survivalStats.maxHealth);
        UpdateStatUI(hungerBar, survivalStats.hunger / survivalStats.maxHunger);

        if (survivalStats.GetInjurySeverity == InjurySeverity.Minor)
        {
            StartBlinkingInjuryIcon();
        }
        else if (survivalStats.GetInjurySeverity == InjurySeverity.Major)
        {
            StartRedWhiteBlinkingInjuryIcon();
        }
    }

    private void UpdateStatUI(Image statBar, float fillAmount)
    {
        statBar.fillAmount = fillAmount;
        statBar.color = fillAmount <= warningThreshold ? warningColor : normalColor;
    }

    // 부상 아이콘을 노란색과 흰색으로 깜박이게 하는 함수
    public void StartBlinkingInjuryIcon()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            currentBlinkCoroutine = StartCoroutine(BlinkInjuryIcon(Color.yellow, Color.white));
        }
    }

    // 부상 아이콘을 빨간색과 흰색으로 깜박이게 하는 함수
    public void StartRedWhiteBlinkingInjuryIcon()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            currentBlinkCoroutine = StartCoroutine(BlinkInjuryIcon(Color.red, Color.white));
        }
    }

    // 부상 아이콘 깜박임 중지 함수
    public void StopBlinkingInjuryIcon()
    {
        isBlinking = false;
        injuryIcon.color = normalColor; // 기본 색상으로 복귀

        if (currentBlinkCoroutine != null)
        {
            StopCoroutine(currentBlinkCoroutine);
            currentBlinkCoroutine = null;
        }
    }

    // 주어진 두 색상으로 깜박이는 코루틴
    private IEnumerator BlinkInjuryIcon(Color color1, Color color2)
    {
        while (isBlinking)
        {
            injuryIcon.color = color1;
            yield return new WaitForSeconds(blinkInterval);

            injuryIcon.color = color2;
            yield return new WaitForSeconds(blinkInterval);
        }
    }
}

