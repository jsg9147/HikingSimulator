using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum WeatherType { Clear, Rain, Fog, Heatwave }
public class WeatherManager : SingletonBase<WeatherManager>
{
    public WeatherType currentWeather = WeatherType.Clear;
    public WeatherType nextWeather = WeatherType.Clear;

    public RainController rainEffectPrefab; // 비 효과 오브젝트
    private RainController rainEffect;

    public float minWeatherChangeInterval = 30f; // 날씨 변경 최소 주기 (초 단위)
    public float maxWeatherChangeInterval = 60f; // 날씨 변경 최대 주기 (초 단위)
    private float weatherChangeCounter = 0f;
    private float weatherChangeInterval;

    private LightManager lightManager;
    private TimeManager timeManager;

    void Start()
    {
        lightManager = FindObjectOfType<LightManager>();
        timeManager = FindObjectOfType<TimeManager>();

        SetRandomWeatherChangeInterval();
    }

    void Update()
    {
        weatherChangeCounter += Time.deltaTime;
        if (weatherChangeCounter >= weatherChangeInterval)
        {
            weatherChangeCounter = 0f;
            ChangeToNextWeather();
            SetRandomWeatherChangeInterval();
            SetRandomWeather();
        }
    }

    private void SetRandomWeather()
    {
        // 각 날씨별 출현 확률을 설정합니다. (Clear 50%, Rain 20%, Fog 15%, Heatwave 15%)
        float[] weatherProbabilities = new float[] { 0.5f, 0.15f, 0.2f, 0.15f };
        float randomValue = Random.Range(0f, 1f);
        float cumulativeProbability = 0f;

        // 확률에 따라 다음 날씨를 설정합니다.
        for (int i = 0; i < weatherProbabilities.Length; i++)
        {
            cumulativeProbability += weatherProbabilities[i];
            if (randomValue <= cumulativeProbability)
            {
                nextWeather = (WeatherType)i;
                break;
            }
        }

        Debug.Log("다음 날씨: " + nextWeather);
    }


    private void ChangeToNextWeather()
    {
        StartCoroutine(ChangeWeather(nextWeather));
    }

    private IEnumerator ChangeWeather(WeatherType newWeather)
    {
        // 현재 날씨가 새 날씨와 같으면 반환
        if (currentWeather == newWeather) yield break;

        // 현재 날씨 종료
        switch (currentWeather)
        {
            case WeatherType.Rain:
                yield return StartCoroutine(StopRain());
                break;
            case WeatherType.Fog:
                StopFog();
                break;
            case WeatherType.Heatwave:
                StopHeatwave();
                break;
        }

        // 새 날씨 시작
        switch (newWeather)
        {
            case WeatherType.Rain:
                yield return StartCoroutine(StartRain());
                break;
            case WeatherType.Fog:
                StartFog();
                break;
            case WeatherType.Heatwave:
                StartHeatwave();
                break;
        }

        currentWeather = newWeather;
        Debug.Log("현재 날씨: " + currentWeather);
    }

    private IEnumerator StartRain()
    {
        rainEffect.gameObject.SetActive(true);
        lightManager.AdjustLightForRain();
        yield return null;
    }

    private IEnumerator StopRain()
    {
        rainEffect.gameObject.SetActive(false);
        lightManager.RestoreOriginalLight();
        yield return null;
    }

    private void StartFog()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.7f, 0.7f, 0.7f);
        RenderSettings.fogDensity = 0.1f;
        lightManager.AdjustLightForFog();
    }

    private void StopFog()
    {
        RenderSettings.fog = false;
        lightManager.RestoreOriginalLight();
    }

    private void StartHeatwave()
    {
        lightManager.AdjustLightForHeatwave();
    }

    private void StopHeatwave()
    {
        lightManager.RestoreOriginalLight();
    }

    private void SetRandomWeatherChangeInterval()
    {
        weatherChangeInterval = Random.Range(minWeatherChangeInterval, maxWeatherChangeInterval);
        Debug.Log("새 날씨 변경 주기: " + weatherChangeInterval + "초");
    }

    public float DayDuration()
    {
        return timeManager.dayDuration;
    }

    public string GetFormattedTime()
    {
        return timeManager.GetFormattedTime();
    }

    public float GetTotalElapsedTime()
    {
        return timeManager.GetTotalElapsedTime();
    }

    public void LightSetup()
    {
        if(lightManager != null)
            lightManager.SetupDirectionalLight();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LightSetup();
        rainEffect = Instantiate(rainEffectPrefab);
        rainEffect.gameObject.SetActive(false);
        rainEffect.SetPlayer(GameManager.instance.playerTransfrom);
    }

    public void SkipTime()
    {
        timeManager.SkipRandomTime();
    }

    public int GetMidnightCount()
    {
        return timeManager.GetMidnightCount();
    }
}

public enum TimeOfDay { Day, Night }