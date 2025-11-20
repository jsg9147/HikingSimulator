using UnityEngine;
using System.Collections;

public class LightManager : MonoBehaviour
{
    public Light directionalLightPrefab; // 주 광원의 프리팹
    private Light directionalLight; // 주 광원

    private Color originalLightColor;
    private float originalLightIntensity;

    public float transitionDuration = 5f; // 낮에서 밤으로 전환되는 시간 (초 단위)
    private bool isTransitioning = false; // 전환 중인지 여부

    public TimeOfDay currentTimeOfDay = TimeOfDay.Day;

    public Transform sunPivot; // 태양 광원의 회전 중심점 (없으면 WorldOrigin 사용)
    public float dayAngleRange = 120f; // 태양이 움직이는 각도 (0이 정오, 180이 자정)

    void Awake()
    {
        SetupDirectionalLight();
    }

    void Start()
    {
        originalLightColor = directionalLight.color;
        originalLightIntensity = directionalLight.intensity;
    }

    public void SetupDirectionalLight()
    {
        Light[] existingLights = FindObjectsOfType<Light>();
        foreach (Light light in existingLights)
        {
            if (light.type == LightType.Directional && light != directionalLight)
            {
                Destroy(light.gameObject);
            }
        }

        if (directionalLight == null)
        {
            directionalLight = Instantiate(directionalLightPrefab);
            DontDestroyOnLoad(directionalLight.gameObject);
        }

        originalLightColor = directionalLight.color;
        originalLightIntensity = directionalLight.intensity;
    }

    public void UpdateLightRotation(float timeProgress)
    {
        if (sunPivot == null)
        {
            sunPivot = new GameObject("SunPivot").transform;
            directionalLight.transform.SetParent(sunPivot);
            directionalLight.transform.localPosition = Vector3.zero;
        }

        // 0도에서 360도가 아닌 -90도에서 270도로 설정하여 해가 뜨고 지는 시간에 맞춘다.
        float angle = Mathf.Lerp(-90f, 270f, timeProgress);
        sunPivot.rotation = Quaternion.Euler(angle, 0f, 0f);

        // 태양의 위치에 따라 조명의 색상과 강도 조정
        UpdateLightForTimeOfDay(timeProgress);
    }

    public void UpdateLightForTimeOfDay(float timeProgress)
    {
        // 조명 색상 및 강도를 시간에 따라 동적으로 조정
        if (timeProgress >= 0.25f && timeProgress <= 0.75f)
        {
            // 낮 (6시 ~ 18시)
            ResetVisibility();
            //ResetCameraView(mainCamera);
            directionalLight.color = Color.Lerp(new Color(1f, 0.5f, 0.3f), originalLightColor, Mathf.InverseLerp(0.25f, 0.5f, timeProgress));
            directionalLight.intensity = Mathf.Lerp(0.1f, originalLightIntensity, Mathf.InverseLerp(0.25f, 0.5f, timeProgress));
        }
        else
        {
            // 밤 (18시 이후 ~ 6시 이전)
            SetNightVisibility();
            //SetNightCameraView(mainCamera);
            directionalLight.color = Color.Lerp(originalLightColor, new Color(0.05f, 0.05f, 0.1f), Mathf.InverseLerp(0.75f, 1f, timeProgress));
            directionalLight.intensity = Mathf.Lerp(originalLightIntensity, 0.1f, Mathf.InverseLerp(0.75f, 1f, timeProgress));
        }
    }

    public IEnumerator ToggleDayNight(TimeOfDay newTimeOfDay)
    {
        isTransitioning = true;
        currentTimeOfDay = newTimeOfDay;

        float elapsedTime = 0f;
        Color startColor = directionalLight.color;
        float startIntensity = directionalLight.intensity;
        Color targetColor;
        float targetIntensity;

        if (newTimeOfDay == TimeOfDay.Night)
        {
            targetColor = new Color(0.05f, 0.05f, 0.1f); // 더 어두운 색상
            targetIntensity = 0.1f; // 더 낮은 강도
        }
        else
        {
            targetColor = originalLightColor;
            targetIntensity = originalLightIntensity;
        }

        while (elapsedTime < transitionDuration)
        {
            directionalLight.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionDuration);
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        directionalLight.color = targetColor;
        directionalLight.intensity = targetIntensity;

        isTransitioning = false;
    }

    public void ApplyTimeOfDay()
    {
        if (currentTimeOfDay == TimeOfDay.Night)
        {
            directionalLight.color = new Color(0.05f, 0.05f, 0.1f); // 더 어두운 색상
            directionalLight.intensity = 0.1f; // 더 낮은 강도
        }
        else
        {
            directionalLight.color = originalLightColor;
            directionalLight.intensity = originalLightIntensity;
        }
    }

    public void AdjustLightForRain()
    {
        StartCoroutine(AdjustLightForWeather(new Color(0.5f, 0.5f, 0.5f)));
    }

    public void AdjustLightForFog()
    {
        StartCoroutine(AdjustLightForWeather(new Color(0.6f, 0.6f, 0.6f)));
    }

    public void AdjustLightForHeatwave()
    {
        StartCoroutine(AdjustLightForWeather(new Color(1f, 0.5f, 0f)));
    }

    private IEnumerator AdjustLightForWeather(Color targetColor)
    {
        Color startColor = directionalLight.color;
        float startIntensity = directionalLight.intensity;
        float targetIntensity = currentTimeOfDay == TimeOfDay.Night ? 0.2f : originalLightIntensity;

        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            directionalLight.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionDuration);
            directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        directionalLight.color = targetColor;
        directionalLight.intensity = targetIntensity;
    }

    public void RestoreOriginalLight()
    {
        if (currentTimeOfDay == TimeOfDay.Night)
        {
            directionalLight.color = new Color(0.05f, 0.05f, 0.1f); // 더 어두운 색상
            directionalLight.intensity = 0.1f; // 더 낮은 강도
        }
        else
        {
            directionalLight.color = originalLightColor;
            directionalLight.intensity = originalLightIntensity;
        }
    }
    private void SetNightVisibility()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.05f, 0.05f, 0.1f);
        RenderSettings.fogStartDistance = 10f;
        RenderSettings.fogEndDistance = 50f;
        CameraController.instance.ToggleDayNight(true);
        //StartCoroutine(AdjustCameraForNight());
    }

    private void ResetVisibility()
    {
        RenderSettings.fog = false;
        CameraController.instance.ToggleDayNight(false);
        //StartCoroutine(ResetCameraView());
    }
    //private IEnumerator AdjustCameraForNight()
    //{
    //    float initialClipPlane = CameraController.instance.GetComponent<Camera>().farClipPlane;
    //    float targetClipPlane = 50f; // 밤에 가시거리 제한

    //    float elapsedTime = 0f;
    //    while (elapsedTime < transitionDuration)
    //    {
    //        float currentClipPlane = Mathf.Lerp(initialClipPlane, targetClipPlane, elapsedTime / transitionDuration);
    //        CameraController.instance.GetComponent<Camera>().farClipPlane = currentClipPlane;
    //        elapsedTime += Time.deltaTime;
    //        print(currentClipPlane);
    //        yield return null;
    //    }

    //    CameraController.instance.GetComponent<Camera>().farClipPlane = targetClipPlane;
    //}

    //private IEnumerator ResetCameraView()
    //{
    //    float initialClipPlane = CameraController.instance.GetComponent<Camera>().farClipPlane;
    //    float targetClipPlane = 1000f; // 낮에 기본 가시거리로 복원

    //    float elapsedTime = 0f;
    //    while (elapsedTime < transitionDuration)
    //    {
    //        float currentClipPlane = Mathf.Lerp(initialClipPlane, targetClipPlane, elapsedTime / transitionDuration);
    //        CameraController.instance.GetComponent<Camera>().farClipPlane = currentClipPlane;
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    CameraController.instance.GetComponent<Camera>().farClipPlane = targetClipPlane;
    //}
}
