using UnityEngine;

public class GameExit : MonoBehaviour
{
    // 이 메서드를 호출하면 게임이 종료됩니다.
    public void ExitGame()
    {
#if UNITY_EDITOR
        // 에디터에서 실행 중인 경우 에디터를 종료합니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서 실행 중인 경우 게임을 종료합니다.
            Application.Quit();
#endif
    }
}
