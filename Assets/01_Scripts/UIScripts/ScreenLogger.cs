using UnityEngine;
using System.Collections.Generic;

public class ScreenLogger : MonoBehaviour
{
    private static ScreenLogger instance;
    private List<string> logMessages = new List<string>();
    private Vector2 scrollPosition;
    private GUIStyle logStyle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Application.logMessageReceived += HandleLog;

            logStyle = new GUIStyle();
            logStyle.fontSize = 20; // 폰트 크기 설정
            logStyle.normal.textColor = Color.white; // 텍스트 색상 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            Application.logMessageReceived -= HandleLog;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        AddMessage(logString);
    }

    private void AddMessage(string message)
    {
        logMessages.Add(message);
        if (logMessages.Count > 10) // 화면에 표시할 최대 로그 메시지 수
        {
            logMessages.RemoveAt(0);
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical(GUILayout.Width(Screen.width));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(Screen.width), GUILayout.Height(Screen.height / 2));
        for (int i = 0; i < logMessages.Count; i++)
        {
            GUILayout.Label(logMessages[i], logStyle);
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    public static void Log(string message)
    {
        Debug.Log(message);
        if (instance != null)
        {
            instance.AddMessage(message);
        }
    }
}
