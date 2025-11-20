using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamAchievement : MonoBehaviour
{
    private const string ACH_START_GAME = "ACH_START_GAME"; // 스팀에서 설정한 도전과제 ID
    private const string ACH_ENDING_GAME = "ACH_ENDING_GAME"; // 스팀에서 설정한 도전과제 ID

    public bool isStart;

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("SteamManager가 초기화되지 않음");
            return;
        }

        if(isStart) 
        {
            GameStart();
        }
    }

    // 도전과제 달성 함수
    public void UnlockAchievement(string achievementID)
    {
        if (!SteamManager.Initialized)
            return;

        SteamUserStats.SetAchievement(achievementID);
        SteamUserStats.StoreStats(); // 스팀 서버에 업데이트
    }

    // 예시: 게임에서 승리할 때 도전과제를 달성하는 코드
    public void GameStart()
    {
        UnlockAchievement(ACH_START_GAME);
    }

    public void GameEnding()
    {
        UnlockAchievement(ACH_ENDING_GAME);
    }

    public bool IsAchievementUnlocked(string achievementID)
    {
        bool isUnlocked = false;
        SteamUserStats.GetAchievement(achievementID, out isUnlocked);
        return isUnlocked;
    }

}
