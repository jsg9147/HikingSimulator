using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenualPopup : MonoBehaviour
{
    [TextArea] public string[] menualStrings; // 여러 문자열을 저장할 배열
    public GameObject popup; // 팝업 창
    public TMP_Text menualText; // TMP_Text 컴포넌트
    public TMP_Text pageText;
    public Color lastLineColor = Color.yellow;

    private int currentPage = 0; // 현재 페이지 인덱스

    private void Start()
    {
        // 팝업을 처음 켰을 때 첫 번째 문자열을 표시
        if (menualStrings.Length > 0)
        {
            UpdateTextDisplay();
        }
    }

    public void PopupOff()
    {
        popup.SetActive(false);
    }

    public void PopupToggle()
    {
        if (popup.activeSelf)
        {
            PopupOff();
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
        else
        {
            UIManager.instance.OpenUI(popup);
            UIManager.instance.CursorChange(CursorLockMode.None);
        }
    }

    public void NextPage()
    {
        if (currentPage < menualStrings.Length - 1) // 마지막 페이지가 아닌 경우
        {
            currentPage++; // 페이지 인덱스를 하나 증가
            UpdateTextDisplay(); // 텍스트 업데이트
        }

        pageText.text = $"{currentPage + 1} / {menualStrings.Length}";
    }

    public void PreviousPage()
    {
        if (currentPage > 0) // 첫 번째 페이지가 아닌 경우
        {
            currentPage--; // 페이지 인덱스를 하나 감소
            UpdateTextDisplay(); // 텍스트 업데이트
        }
        pageText.text = $"{currentPage + 1} / {menualStrings.Length}";
    }

    private void UpdateTextDisplay()
    {
        // 2번째 페이지일 때만 마지막 줄을 노란색으로 변경
        if (currentPage == 1) // 두 번째 페이지 (0-based index)
        {
            menualText.text = ApplyColorToLastLine(menualStrings[currentPage], lastLineColor);
        }
        else
        {
            menualText.text = menualStrings[currentPage]; // 다른 페이지는 그냥 출력
        }
    }

    private string ApplyColorToLastLine(string text, Color color)
    {
        // 텍스트를 줄 단위로 나누기
        string[] lines = text.Split('\n');

        // 마지막 줄에 색상 적용
        if (lines.Length > 0)
        {
            lines[lines.Length - 1] = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{lines[lines.Length - 1]}</color>";
        }

        // 줄들을 다시 합쳐서 반환
        return string.Join("\n", lines);
    }
}
