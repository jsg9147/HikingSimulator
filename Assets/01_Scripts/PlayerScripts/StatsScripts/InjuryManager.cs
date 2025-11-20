using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InjuryManager
{
    public InjurySeverity armInjury = InjurySeverity.None;
    public InjurySeverity legInjury = InjurySeverity.None;
    public InjurySeverity torsoInjury = InjurySeverity.None;

    public InjurySeverity injury = InjurySeverity.None;

    private Coroutine armInjuryCoroutine;
    private Coroutine legInjuryCoroutine;
    private Coroutine torsoInjuryCoroutine;
    private Coroutine injuryEffectCoroutine;

    private Image injuryEffectImage;

    public void Initialize(Image effectImage)
    {
        injuryEffectImage = effectImage;
    }

    public void TryRandomInjury(float duration)
    {
        float randomValue = Random.Range(0f, 1f);
        if (randomValue < 0.33f && armInjury == InjurySeverity.None)
        {   
            InjurePlayer(SurvivalStats.Arm, InjurySeverity.Minor, duration);
        }
        else if (randomValue < 0.66f && legInjury == InjurySeverity.None)
        {
            InjurePlayer(SurvivalStats.Leg, InjurySeverity.Minor, duration);
        }
        else if (torsoInjury == InjurySeverity.None)
        {
            InjurePlayer(SurvivalStats.Torso, InjurySeverity.Minor, duration);
        }
    }

    public void InjurePlayer(string bodyPart, InjurySeverity severity, float duration)
    {
        switch (bodyPart)
        {
            case SurvivalStats.Arm:
                if (armInjury == InjurySeverity.None)
                {
                    armInjury = severity;
                    NotifyInjury(severity);
                    if (severity == InjurySeverity.Minor)
                    {
                        if (armInjuryCoroutine != null)
                        {
                            SurvivalStatsManager.instance.StopCoroutine(armInjuryCoroutine);
                        }
                        armInjuryCoroutine = SurvivalStatsManager.instance.StartCoroutine(UpgradeInjuryAfterDelay(SurvivalStats.Arm, duration));
                    }
                }
                break;
            case SurvivalStats.Leg:
                if (legInjury == InjurySeverity.None)
                {
                    legInjury = severity;
                    NotifyInjury(severity);
                    if (severity == InjurySeverity.Minor)
                    {
                        if (legInjuryCoroutine != null)
                        {
                            SurvivalStatsManager.instance.StopCoroutine(legInjuryCoroutine);
                        }
                        legInjuryCoroutine = SurvivalStatsManager.instance.StartCoroutine(UpgradeInjuryAfterDelay(SurvivalStats.Leg, duration));
                    }
                }
                break;
            case SurvivalStats.Torso:
                if (torsoInjury == InjurySeverity.None)
                {
                    torsoInjury = severity;
                    NotifyInjury(severity);
                    if (severity == InjurySeverity.Minor)
                    {
                        if (torsoInjuryCoroutine != null)
                        {
                            SurvivalStatsManager.instance.StopCoroutine(torsoInjuryCoroutine);
                        }
                        torsoInjuryCoroutine = SurvivalStatsManager.instance.StartCoroutine(UpgradeInjuryAfterDelay(SurvivalStats.Torso, duration));
                    }
                }
                break;
            default:
                Debug.Log("알 수 없는 부위: " + bodyPart);
                break;
        }

        GameManager.instance.survivalStatsManager.InjuryCountReset();
        SurvivalStatsManager.instance.survivalStats.ResetInjuryCount();
    }

    private IEnumerator UpgradeInjuryAfterDelay(string bodyPart, float delay)
    {
        yield return new WaitForSeconds(delay);
        UpgradeInjury(bodyPart);
    }

    private void UpgradeInjury(string bodyPart)
    {
        switch (bodyPart)
        {
            case SurvivalStats.Arm:
                if (armInjury == InjurySeverity.Minor)
                {
                    armInjury = InjurySeverity.Major;
                    NotifyInjury(InjurySeverity.Major);
                }
                break;
            case SurvivalStats.Leg:
                if (legInjury == InjurySeverity.Minor)
                {
                    legInjury = InjurySeverity.Major;
                    NotifyInjury(InjurySeverity.Major);
                }
                break;
            case SurvivalStats.Torso:
                if (torsoInjury == InjurySeverity.Minor)
                {
                    torsoInjury = InjurySeverity.Major;
                    NotifyInjury(InjurySeverity.Major);
                }
                break;
        }
    }

    public void HealInjury(InjurySeverity severity)
    {
        if (severity == InjurySeverity.Minor)
        {
            if (armInjury == InjurySeverity.Minor)
            {
                armInjury = InjurySeverity.None;
                Debug.Log("플레이어의 팔 경상이 회복되었습니다.");
            }
            if (legInjury == InjurySeverity.Minor)
            {
                legInjury = InjurySeverity.None;
                Debug.Log("플레이어의 다리 경상이 회복되었습니다.");
            }
            if (torsoInjury == InjurySeverity.Minor)
            {
                torsoInjury = InjurySeverity.None;
                Debug.Log("플레이어의 몸통 경상이 회복되었습니다.");
            }
        }
        else if (severity == InjurySeverity.Major)
        {
            if (armInjury == InjurySeverity.Major)
            {
                armInjury = InjurySeverity.None;
                Debug.Log("플레이어의 팔 중상이 완화되었습니다.");
            }
            if (legInjury == InjurySeverity.Major)
            {
                legInjury = InjurySeverity.None;
                Debug.Log("플레이어의 다리 중상이 완화되었습니다.");
            }
            if (torsoInjury == InjurySeverity.Major)
            {
                torsoInjury = InjurySeverity.None;
                Debug.Log("플레이어의 몸통 중상이 완화되었습니다.");
            }
        }

        if (armInjury == InjurySeverity.None && legInjury == InjurySeverity.None && torsoInjury == InjurySeverity.None)
        {
            injury = InjurySeverity.None;
            UIManager.instance.StopInjuryEffect();
        }
    }

    private void NotifyInjury(InjurySeverity severity)
    {
        injury = severity;
        if (severity == InjurySeverity.Minor || severity == InjurySeverity.Major)
        {
            //StartInjuryEffect(severity);
        }

        if (severity == InjurySeverity.Minor)
        {
            Debug.Log("플레이어가 경상을 입었습니다.");
        }
        else if (severity == InjurySeverity.Major)
        {
            UIManager.instance.StopInjuryEffect();
            Debug.Log("플레이어가 중상을 입었습니다.");
        }
    }

    public void UpgradeAllMinorInjuriesToMajor()
    {
        if (armInjury == InjurySeverity.Minor)
        {
            armInjury = InjurySeverity.Major;
            NotifyInjury(InjurySeverity.Major);
            Debug.Log("플레이어의 팔 부상이 중상으로 바뀌었습니다.");
        }

        if (legInjury == InjurySeverity.Minor)
        {
            legInjury = InjurySeverity.Major;
            NotifyInjury(InjurySeverity.Major);
            Debug.Log("플레이어의 다리 부상이 중상으로 바뀌었습니다.");
        }

        if (torsoInjury == InjurySeverity.Minor)
        {
            torsoInjury = InjurySeverity.Major;
            NotifyInjury(InjurySeverity.Major);
            Debug.Log("플레이어의 몸통 부상이 중상으로 바뀌었습니다.");
        }
    }

}
public enum InjurySeverity { None, Minor, Major }