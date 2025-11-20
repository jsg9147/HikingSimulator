using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTransition : MonoBehaviour
{

    // 씬 이름을 저장할 변수
    public string sceneName;

    // 트리거 콜라이더에 진입했을 때 호출되는 메서드
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 콜라이더에 진입했는지 확인
        if (other.CompareTag("Player") && !string.IsNullOrEmpty(sceneName))
        {
            // 설정된 씬으로 이동
            SceneManager.LoadScene(sceneName);
        }
    }
}
