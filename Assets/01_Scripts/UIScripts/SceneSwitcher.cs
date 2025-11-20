using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    public Button yourButton; // Inspector에서 설정할 버튼
    public string sceneName;  // 전환할 씬의 이름

    void Start()
    {
        // 버튼의 onClick 이벤트에 메서드 추가
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(SwitchScene);
        }
    }

    // 씬 전환 메서드
    public void SwitchScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
