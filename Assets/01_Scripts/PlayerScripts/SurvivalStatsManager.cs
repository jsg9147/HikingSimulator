using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SurvivalStatsManager : SingletonBase<SurvivalStatsManager>
{
    public SurvivalStats survivalStats;
    public InjuryManager injuryManager;
    public ConsumableManager consumableManager;

    private PlayerMovement playerMovement;
    private PlayerAnimator playerAnimator;

    private float normalRecoveryRate = 0f; // 일반 회복 속도
    private float tentRecoveryRate = 3f; // 텐트에서의 회복 속도
    private int tentSetupCount = 0; // 텐트 설치 횟수
    private int pedometer = 0;

    private int injuryPedometer = 0;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<PlayerAnimator>();

        injuryManager = new InjuryManager();
        survivalStats = new SurvivalStats(injuryManager);
        consumableManager = new ConsumableManager(survivalStats, injuryManager);

        survivalStats.SetRecoveryRate(normalRecoveryRate); // 기본 회복 속도 설정

    }

    void Update()
    {
        survivalStats.UpdateStats(Time.deltaTime);
        UIManager.instance.UpdateSurvivalStats(survivalStats);

        playerMovement.AdjustMoveSpeed(survivalStats.health); // 체력에 따른 이동 속도 조절
        playerAnimator.SetWalkingSpeed(playerMovement.GetCurrentSpeedRatio()); // 걷는 속도에 따른 애니메이션 속도 조절
    }

    public void EnterTent()
    {
        survivalStats.SetRecoveryRate(tentRecoveryRate); // 텐트 회복 속도로 설정
        survivalStats.SetRestState(true); // 텐트 회복 속도로 설정
    }

    public void ExitTent()
    {
        survivalStats.SetRestState(false); // 일반 회복 속도로 설정
    }

    public void ExitLodge()
    {
        survivalStats.SetRecoveryRate(normalRecoveryRate); // 일반 회복 속도로 설정
    }

    public void Recovery()
    {
        survivalStats.RestoreHealth(100);
    }

    public void IncrementTentSetupCount()
    {
        tentSetupCount++;
        Debug.Log("텐트를 설치한 횟수: " + tentSetupCount);

        if (tentSetupCount >= 10 && injuryManager.armInjury == InjurySeverity.None)
        {
            injuryManager.InjurePlayer(SurvivalStats.Arm, InjurySeverity.Minor, WeatherManager.instance.DayDuration());
        }
    }

    public void IncrementStepCount()
    {
        pedometer++;
        injuryPedometer++;
        if (injuryPedometer >= 7000 && injuryManager.legInjury == InjurySeverity.None)
        {
            injuryManager.InjurePlayer(SurvivalStats.Leg, InjurySeverity.Minor, WeatherManager.instance.DayDuration());
        }
    }

    public void InjuryCountReset()
    {
        injuryPedometer = 0;
        tentSetupCount = 0;
    }

    public int Pedometer()
    {
        return pedometer;
    }

    public void Recovery(ItemData itemData)
    {
        StartCoroutine(RecoveryCoroutine(itemData));
    }

    IEnumerator RecoveryCoroutine(ItemData itemData)
    {
        yield return new WaitForSeconds(0.15f);

        if (itemData.healthRestore > 0)
        {
            survivalStats.RestoreHealth(itemData.healthRestore);
            MasterAudio.PlaySound3DAtTransform("Drinking", GameManager.instance.playerTransfrom);
        }

        if (itemData.hungerRestore > 0)
        {
            survivalStats.RestoreHunger(itemData.hungerRestore);
            MasterAudio.PlaySound3DAtTransform("Eating", GameManager.instance.playerTransfrom);
        }
    }
}
