using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float dayDuration = 60f; // 낮의 길이 (초 단위)
    private float timeCounter = 0f;
    private float totalElapsedTime = 0f; // 총 경과 시간을 기록하는 변수
    private float skippedTime = 0f; // SkipRandomTime으로 스킵된 시간을 기록하는 변수

    public TimeOfDay currentTimeOfDay = TimeOfDay.Day;

    public float sunriseHour = 6f; // 일출 시간 (24시간 형식)
    public float sunsetHour = 18f; // 일몰 시간 (24시간 형식)

    private LightManager lightManager;

    private int midnightCount = 0; // 00시가 지나간 횟수를 기록
    private bool midnightPasses = false; // 00시가 지났는지 체크하는 변수

    float injuryTimeCheck = 0f;
    bool injuryStart = false;
    bool upgradeInjury = false;
    void Start()
    {
        lightManager = FindObjectOfType<LightManager>();

        // 6시(일출 시간)에 시작하도록 시간 초기화
        float initialHour = 6f; // 6시로 설정
        timeCounter = (initialHour / 24f) * dayDuration; // 24시간 기준으로 변환하여 timeCounter에 설정
    }

    void Update()
    {
        // 시간 진행
        float deltaTime = Time.deltaTime;
        timeCounter += deltaTime;
        totalElapsedTime += deltaTime; // 총 경과 시간도 함께 증가
        if (timeCounter >= dayDuration)
        {
            timeCounter = 0f;
        }

        // 자정이 지났는지 체크
        CheckMidnightPass();

        // 현재 시간에 따라 조명 회전 업데이트
        float timeProgress = timeCounter / dayDuration;
        lightManager.UpdateLightRotation(timeProgress);

        if (!injuryStart && SurvivalStatsManager.instance.injuryManager.injury == InjurySeverity.Minor)
        {
            InjuryStart();
        }

        if (SurvivalStatsManager.instance.injuryManager.injury == InjurySeverity.None)
        {
            injuryStart = false;
            injuryTimeCheck = 0f;
            upgradeInjury = false;
        }

        if (injuryStart && !upgradeInjury)
        {
            if (injuryTimeCheck + dayDuration * 3f <= GetTotalElapsedTime())
            {
                SurvivalStatsManager.instance.injuryManager.UpgradeAllMinorInjuriesToMajor();
                upgradeInjury = true;
            }
        }
    }

    public void InjuryStart()
    {
        injuryTimeCheck = GetTotalElapsedTime();
        injuryStart = true;
    }

    private void CheckMidnightPass()
    {
        // 현재 시간을 계산
        float timeProgress = timeCounter / dayDuration;
        int currentHour = Mathf.FloorToInt(timeProgress * 24);

        // 자정(00시)을 지나갔을 때
        if (currentHour == 0 && !midnightPasses)
        {
            midnightCount++;
            midnightPasses = true;
        }
        // 자정을 지나지 않은 경우에만 reset
        else if (currentHour > 0)
        {
            midnightPasses = false;
        }
    }

    public string GetFormattedTime()
    {
        float timeProgress = timeCounter / dayDuration;
        int totalMinutes = Mathf.FloorToInt(timeProgress * 24 * 60);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }

    public int GetMidnightCount()
    {
        return midnightCount;
    }

    public void SkipRandomTime()
    {
        // 6시간에서 8시간 사이의 랜덤한 시간을 계산
        float randomHours = Random.Range(8f, 10f);

        // 해당 시간을 초로 변환하여 timeCounter에 추가
        float timeToAdd = (randomHours / 24f) * dayDuration;
        timeCounter += timeToAdd;
        skippedTime += timeToAdd; // 스킵된 시간을 누적

        // 시간 초과가 발생하면 24시간 주기로 순환하도록 처리
        if (timeCounter >= dayDuration)
        {
            timeCounter -= dayDuration;
            midnightCount++;
        }

        // 현재 시간에 따라 조명 회전 업데이트
        float timeProgress = timeCounter / dayDuration;
        lightManager.UpdateLightRotation(timeProgress);
    }

    public float GetTotalElapsedTime()
    {
        return totalElapsedTime + skippedTime; // 총 경과 시간 + 스킵된 시간
    }

    public float GetSkippedTime()
    {
        return skippedTime; // 스킵된 시간 반환
    }
}
