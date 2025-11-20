using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SmartWatchUI : MonoBehaviour
{
    public static SmartWatchUI instance;
    public TMP_Text pedometerText;
    public TMP_Text distanceText;
    public Image weatherImage; // 변경된 부분
    public TMP_Text timeText;
    public TMP_Text heartRateText;

    public TMP_Text dateText;

    public Sprite clearSprite;
    public Sprite rainSprite;
    public Sprite fogSprite;
    public Sprite heatwaveSprite;

    private float heartRateUpdateTime = 1.0f; // 심박수 갱신 주기 (초)
    private float heartRateTimer;
    private float currentHeartRate;
    private float targetHeartRate;

    private Vector3 miniPosition;
    private Vector3 miniScale;
    private Vector3 enlargedPosition;
    private Vector3 enlargedScale;
    private bool isEnlarged = false;

    private RectTransform rectTransform;
    private Vector3 targetPosition;
    private Vector3 targetScale;
    private float transitionSpeed = 4.0f; // 애니메이션 속도

    private Dictionary<WeatherType, Sprite> weatherIcons;

    private Transform currentParent;
    private Transform canvasTransform;

    private void Start()
    {
        canvasTransform = (GameObject.Find("Canvas").transform);
        currentParent = transform.parent;
        rectTransform = GetComponent<RectTransform>();

        // 초기 상태 저장
        miniPosition = rectTransform.anchoredPosition;
        miniScale = rectTransform.localScale;
        enlargedPosition = new Vector3(0, -100, 0); // 확대된 상태의 위치 (중앙)
        enlargedScale = Vector3.one;

        targetPosition = miniPosition;
        targetScale = miniScale;

        GetComponent<Button>().onClick.AddListener(EnlargePhone);
        heartRateTimer = heartRateUpdateTime;
        currentHeartRate = 80.0f; // 초기 심박수 설정
        targetHeartRate = 80.0f; // 초기 목표 심박수 설정
        heartRateText.text = currentHeartRate.ToString("F0") + " bpm";

        // 날씨 아이콘 초기화
        weatherIcons = new Dictionary<WeatherType, Sprite>()
        {
            { WeatherType.Clear, clearSprite },
            { WeatherType.Rain, rainSprite },
            { WeatherType.Fog, fogSprite },
            { WeatherType.Heatwave, heatwaveSprite }
        };
    }

    private void OnEnable()
    {
        if (InputManager.instance != null)
        {
            InputManager.instance.OnSmartPhonePressed += EnlargePhone;
        }
    }

    private void OnDisable()
    {
        if (InputManager.instance != null)
        {
            InputManager.instance.OnSmartPhonePressed -= EnlargePhone;
        }
    }

    void Update()
    {

        if (WeatherManager.instance != null)
        {
            timeText.text = WeatherManager.instance.GetFormattedTime();
            UpdateWeatherIcon(WeatherManager.instance.nextWeather);
        }

        if (GameManager.instance != null)
        {
            pedometerText.text = GameManager.instance.survivalStatsManager.Pedometer().ToString();
            distanceText.text = GameManager.instance.playerMovement.GetTotalDistanceMoved().ToString("F1") + " YD";
        }

        // 심박수 갱신
        heartRateTimer -= Time.deltaTime;
        if (heartRateTimer <= 0f)
        {
            UpdateHeartRate();
            heartRateTimer = heartRateUpdateTime;
        }

        // 현재 심박수를 목표 심박수에 가깝게 서서히 변경
        currentHeartRate = Mathf.Lerp(currentHeartRate, targetHeartRate, Time.deltaTime);
        heartRateText.text = Mathf.RoundToInt(currentHeartRate).ToString() + " bpm";
            

        // 서서히 크기와 위치 변경
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * transitionSpeed);
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * transitionSpeed);

        dateText.text = WeatherManager.instance.GetMidnightCount().ToString() + " Day";
    }

    private void UpdateHeartRate()
    {
        // 75에서 85 사이의 랜덤 목표 심박수 설정
        targetHeartRate = Random.Range(75.0f, 85.0f);
    }

    private void UpdateWeatherIcon(WeatherType weatherType)
    {
        if (weatherIcons.ContainsKey(weatherType))
        {
            weatherImage.sprite = weatherIcons[weatherType];
        }
    }

    void EnlargePhone()
    {
        if (isEnlarged)
        {
            // 작게 만들기
            transform.SetParent(currentParent);
            targetPosition = miniPosition;
            targetScale = miniScale;
        }
        else
        {
            // 크게 만들기
            transform.SetParent(canvasTransform);
            targetPosition = enlargedPosition;
            targetScale = enlargedScale;
        }
        isEnlarged = !isEnlarged;
    }
}