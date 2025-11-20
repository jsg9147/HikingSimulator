using System.Collections.Generic;
using UnityEngine;

public class SingletonManager : MonoBehaviour
{
    private static SingletonManager _instance;
    private List<MonoBehaviour> _singletonList = new List<MonoBehaviour>();

    public static SingletonManager instance
    {
        get
        {
            if (_instance == null)
            {
                // SingletonManager가 이미 존재하는지 확인
                _instance = FindObjectOfType<SingletonManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SingletonManager");
                    _instance = go.AddComponent<SingletonManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // 싱글톤 오브젝트 등록
    public void RegisterSingleton(MonoBehaviour singleton)
    {
        if (!_singletonList.Contains(singleton))
        {
            _singletonList.Add(singleton);
        }
    }

    // 싱글톤 오브젝트 제거
    public void UnregisterSingleton(MonoBehaviour singleton)
    {
        if (_singletonList.Contains(singleton))
        {
            _singletonList.Remove(singleton);
        }
    }

    // 모든 싱글톤 오브젝트 리셋
    public void ResetAllSingletons()
    {
        // 순회를 위해 리스트 복사
        var singletons = new List<MonoBehaviour>(_singletonList);

        foreach (var singleton in singletons)
        {
            if (singleton != null)
            {
                Destroy(singleton.gameObject);
            }
        }
        _singletonList.Clear();
    }

    // SingletonManager가 파괴될 때 모든 싱글톤 정리
    private void OnDestroy()
    {
        if (_instance == this)
        {
            ResetAllSingletons();  // 싱글톤 오브젝트 정리
            _instance = null;
        }
    }
}
