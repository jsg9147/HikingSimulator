using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks; // Steamworks.NET API

public class SteamAchievements : MonoBehaviour
{
    private List<string> achievements = new List<string> {
        "ACH_FIRST_WIN",  // 여기에 Steamworks에서 설정한 업적의 API 이름을 추가
        "ACH_COLLECT_ALL_ITEMS"
    };

    private void Start()
    {
        if (!SteamManager.Initialized) // Steamworks가 초기화되었는지 확인
        {
            Debug.LogError("Steam is not initialized!");
            Application.Quit();
            return;
        }

        Debug.Log("Steam is initialized!");
    }

    // 업적을 해제하는 함수
    public void UnlockAchievement(string achievementID)
    {
        if (!SteamManager.Initialized) // Steamworks가 초기화되었는지 확인
        {
            Debug.LogError("Steam is not initialized!");
            return;
        }

        bool isUnlocked = false;

        // 업적이 이미 해제되었는지 확인
        SteamUserStats.GetAchievement(achievementID, out isUnlocked);
        if (isUnlocked)
        {
            Debug.Log($"Achievement {achievementID} is already unlocked.");
            return;
        }

        // 업적 해제 시도
        SteamUserStats.SetAchievement(achievementID);
        bool success = SteamUserStats.StoreStats();

        if (success)
        {
            Debug.Log($"Achievement {achievementID} unlocked successfully.");
        }
        else
        {
            Debug.LogError($"Failed to unlock achievement {achievementID}.");
        }
    }

    // 특정 업적 해제 예시: 첫 번째 승리 업적 해제
    public void UnlockFirstWinAchievement()
    {
        UnlockAchievement(achievements[0]); // 첫 번째 업적 (ACH_FIRST_WIN) 해제
    }

    // 특정 업적 해제 예시: 모든 아이템 수집 업적 해제
    public void UnlockCollectAllItemsAchievement()
    {
        UnlockAchievement(achievements[1]); // 두 번째 업적 (ACH_COLLECT_ALL_ITEMS) 해제
    }

    // 업적 초기화 (디버그 용도, 모든 업적 리셋)
    public void ResetAchievements()
    {
        if (!SteamManager.Initialized) return;

        foreach (var achievementID in achievements)
        {
            SteamUserStats.ClearAchievement(achievementID);
        }

        SteamUserStats.StoreStats();
        Debug.Log("All achievements reset.");
    }
}
