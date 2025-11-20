using UnityEngine;

[System.Serializable]
public class SurvivalStats
{
    public const string Arm = "Arm";
    public const string Leg = "Leg";
    public const string Torso = "Torso";

    // 스탯 변수
    public float health = 100f; // 체력
    public float hunger = 100f; // 허기짐
    public float thirst = 100f; // 갈증

    // 각 스탯의 최대값
    public float maxHealth = 100f;
    public float maxHunger = 100f;
    public float maxThirst = 100f;

    // 스탯 감소 속도
    public float healthDecayRate = 0.3f;
    public float hungerDecayRate = 0.4f;
    public float thirstDecayRate = 1.5f;

    // 회복 속도
    public float healthRecoveryRate = 0f;

    // 기타 카운트 변수
    private int lowHealthCount = 0; // 체력이 50% 이하로 떨어진 횟수
    private bool lowHealthFlag = false; // 체력이 50 이하로 떨어진 상태 플래그
    private int lowHungerCount = 0; // 허기가 일정 수치 이하로 떨어진 횟수
    private bool isHungerBelowThreshold;
    private float hungerProtectRate = 0;
    private float shoesHealthProtectRate = 0;

    private bool isRest = false;

    private float injuryDecreaseHealthRate = 1.1f;

    private float injuryTimeCheck = 0f;

    // InjuryManager 참조
    private InjuryManager injuryManager;

    public InjurySeverity GetInjurySeverity
    {
        get { return injuryManager.injury; }
    }

    public SurvivalStats(InjuryManager injuryManager)
    {
        this.injuryManager = injuryManager;
    }

    public void ResetInjuryCount()
    {
        lowHealthCount = 0;
        lowHungerCount = 0;
    }

    // 스탯 업데이트 메서드
    public void UpdateStats(float deltaTime)
    {
        DecreaseHunger(deltaTime);
        DecreaseHealth(deltaTime);

        // 체력 50% 이하 체크
        if (health <= 50f)
        {
            if (!lowHealthFlag)
            {
                lowHealthCount++;
                lowHealthFlag = true;
                Debug.Log("체력 감소 횟수 : " + lowHealthCount);
            }

            if (lowHealthCount >= 10 && injuryManager.injury == InjurySeverity.None)
            {
                injuryManager.TryRandomInjury(WeatherManager.instance.DayDuration());
            }
        }
        else
        {
            lowHealthFlag = false;
        }

        if (health <= 0)
        {
            GameManager.instance.GameOver();
        }

        // 각 스탯이 0 이하로 내려가지 않도록 제한
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
        health = Mathf.Clamp(health, 0f, maxHealth);
    }

    // 허기 감소 메서드
    public void DecreaseHunger(float deltaTime)
    {
        float actualHungerDecayRate = hungerDecayRate;

        // 옷 착용 여부를 확인하여 감소 속도 조절
        if (IsWearingClothes())
        {
            actualHungerDecayRate = hungerDecayRate - (hungerDecayRate * hungerProtectRate);

            if (WeatherManager.instance != null)
            {
                if (WeatherManager.instance.currentWeather == WeatherType.Heatwave)
                {
                    actualHungerDecayRate = actualHungerDecayRate  * 1.2f;
                }
            }
        }

        hunger -= actualHungerDecayRate * deltaTime;
        if (hunger <= 0f || thirst <= 0f)
        {
            health -= (maxHealth / 100f) * deltaTime; // 체력 감소 속도 조절
        }
        if (hunger <= 30f) // 허기가 20 이하일 때
        {
            if (!isHungerBelowThreshold)
            {
                lowHungerCount++;
                Debug.Log("허기가 일정 수치 이하로 떨어진 횟수: " + lowHungerCount);
                isHungerBelowThreshold = true;

                if (lowHungerCount > 10)
                {
                    injuryManager.InjurePlayer(Torso, InjurySeverity.Minor, WeatherManager.instance.DayDuration());
                }
            }
        }
        else if (hunger > 20f) // 허기가 20 이상으로 회복될 때
        {
            isHungerBelowThreshold = false;
        }
        hunger = Mathf.Clamp(hunger, 0f, maxHunger);
    }

    // 체력 감소 메서드
    public void DecreaseHealth(float deltaTime)
    {
        float actualHealthDecayRate = healthDecayRate;
        if (isRest)
        {
            if (health < maxHealth)
            {
                health += healthRecoveryRate * deltaTime;
            }
        }
        else
        {
            if (IsWearingShoes())
            {
                actualHealthDecayRate = healthDecayRate - (healthDecayRate * shoesHealthProtectRate);
            }

            if (injuryManager.injury != InjurySeverity.None)
            {
                actualHealthDecayRate = actualHealthDecayRate * injuryDecreaseHealthRate;
            }

            if (WeatherManager.instance != null && WeatherManager.instance.currentWeather == WeatherType.Heatwave)
            {
                actualHealthDecayRate = actualHealthDecayRate * 1.3f;
            }

            health -= actualHealthDecayRate * deltaTime;
        }
    }

    // 아이템 사용 등으로 스탯 회복 메서드
    public void RestoreHealth(float amount)
    {
        if(injuryManager.injury != InjurySeverity.Major)
            health = Mathf.Clamp(health + amount, 0f, maxHealth);
    }

    public void RestoreHunger(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, maxHunger);
    }

    public void RestoreThirst(float amount)
    {
        thirst = Mathf.Clamp(thirst + amount, 0f, maxThirst);
    }

    // 체력 회복 속도 설정 메서드
    public void SetRecoveryRate(float rate)
    {
        healthRecoveryRate = rate;
    }

    public void SetRestState(bool isRest)
    {
        this.isRest = isRest;
    }

    // 옷 착용 여부를 확인하는 메서드
    private bool IsWearingClothes()
    {
        foreach (var equipment in InventoryManager.instance.equipmentSlots)
        {
            if (equipment.GetItem() != null)
            {
                if (equipment.GetItem().itemType == ItemData.ItemType.Equipment && equipment.equipmentType == ItemData.EquipmentType.Clothes)
                {
                    hungerProtectRate = equipment.GetItem().hungerRestore * 0.01f;
                    return true;
                }
            }
        }
        return false;
    }

    // 신발 착용 여부를 확인하는 메서드
    private bool IsWearingShoes()
    {
        foreach (var equipment in InventoryManager.instance.equipmentSlots)
        {
            if (equipment.GetItem() != null)
            {
                if (equipment.GetItem().itemType == ItemData.ItemType.Equipment && equipment.equipmentType == ItemData.EquipmentType.Shoes)
                {
                    shoesHealthProtectRate = equipment.GetItem().healthRestore * 0.01f;
                    return true;
                }
            }
        }
        return false;
    }
}
