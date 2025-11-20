using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Toggle bgmToggle;

    void Start()
    {
        // 토글 상태가 변경될 때마다 OnToggleChanged 함수가 호출되도록 이벤트 등록
        bgmToggle.onValueChanged.AddListener(OnToggleChanged);

        // 초기 토글 상태를 로그로 출력
        Debug.Log("초기 토글 상태: " + bgmToggle.isOn);
    }

    // 토글 상태가 변경될 때 호출되는 함수
    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("토글 ON 상태");
            // 여기에서 토글이 ON일 때 실행할 동작을 정의
        }
        else
        {
            Debug.Log("토글 OFF 상태");
            // 여기에서 토글이 OFF일 때 실행할 동작을 정의
        }
    }
}
