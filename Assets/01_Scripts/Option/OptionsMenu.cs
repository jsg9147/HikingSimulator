using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public GameObject panel;

    // Audio 관련
    public Slider bgmVolumeSlider;
    public Slider sfxVolumeSlider;

    // 해상도 관련
    public TMP_Dropdown resolutionDropdown;
    private List<Resolution> availableResolutions = new List<Resolution>(); // 중복이 제거된 해상도 목록

    // 설정을 저장할 키 값
    private const string BgmVolumeKey = "BGMVolume";
    private const string SfxVolumeKey = "SFXVolume";
    private const string ResolutionIndexKey = "ResolutionIndex";

    void Start()
    {
        panel.SetActive(false);

        // 해상도 목록 가져오기 및 중복 제거
        Resolution[] allResolutions = Screen.resolutions;
        HashSet<string> uniqueResolutions = new HashSet<string>(); // 중복 제거를 위한 HashSet
        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            string option = allResolutions[i].width + " x " + allResolutions[i].height;

            // 중복된 해상도는 추가하지 않음
            if (!uniqueResolutions.Contains(option))
            {
                uniqueResolutions.Add(option);
                availableResolutions.Add(allResolutions[i]);

                // 현재 해상도와 일치하는 해상도 찾기
                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = availableResolutions.Count - 1;
                }
            }
        }

        // 드롭다운 초기화
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (Resolution resolution in availableResolutions)
        {
            options.Add(resolution.width + " x " + resolution.height);
        }

        resolutionDropdown.AddOptions(options);

        // 현재 해상도를 드롭다운에서 선택 상태로 설정
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // 해상도 설정 불러오기
        LoadResolutionSettings();

        // 볼륨 설정 불러오기
        LoadVolumeSettings();

        // 슬라이더가 변경될 때마다 SetBgmVolume, SetSfxVolume 메소드 호출
        bgmVolumeSlider.onValueChanged.AddListener(SetBgmVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);

        // 해상도 드롭다운이 변경될 때마다 SetResolution 호출
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    // 볼륨 설정을 저장하는 메소드
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(BgmVolumeKey, bgmVolumeSlider.value);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolumeSlider.value);
        PlayerPrefs.Save(); // 저장
    }

    // 볼륨 설정을 불러오는 메소드
    private void LoadVolumeSettings()
    {
        // BGM 볼륨 설정 불러오기, 없으면 기본값 1로 설정
        if (PlayerPrefs.HasKey(BgmVolumeKey))
        {
            float savedBgmVolume = PlayerPrefs.GetFloat(BgmVolumeKey);
            bgmVolumeSlider.value = savedBgmVolume;
            MasterAudio.PlaylistMasterVolume = savedBgmVolume;
        }
        else
        {
            bgmVolumeSlider.value = MasterAudio.PlaylistMasterVolume;
        }

        // SFX 볼륨 설정 불러오기, 없으면 기본값 1로 설정
        if (PlayerPrefs.HasKey(SfxVolumeKey))
        {
            float savedSfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey);
            sfxVolumeSlider.value = savedSfxVolume;
            MasterAudio.MasterVolumeLevel = savedSfxVolume;
        }
        else
        {
            sfxVolumeSlider.value = MasterAudio.MasterVolumeLevel;
        }
    }

    // 해상도 설정을 저장하는 메소드
    private void SaveResolutionSettings(int resolutionIndex)
    {
        PlayerPrefs.SetInt(ResolutionIndexKey, resolutionIndex);
        PlayerPrefs.Save(); // 저장
    }

    private void LoadResolutionSettings()
    {
        if (PlayerPrefs.HasKey(ResolutionIndexKey))
        {
            int savedResolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey);
            resolutionDropdown.value = savedResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            SetResolution(savedResolutionIndex); // 저장된 해상도로 설정
        }
        else
        {
            // 기본 해상도를 1600x900으로 설정
            int defaultResolutionIndex = availableResolutions.FindIndex(resolution => resolution.width == 1600 && resolution.height == 900);

            if (defaultResolutionIndex != -1)
            {
                resolutionDropdown.value = defaultResolutionIndex;
                SetResolution(defaultResolutionIndex);
            }
            else
            {
                // 1600x900 해상도가 지원되지 않을 경우, 현재 시스템 해상도로 설정
                resolutionDropdown.value = availableResolutions.Count - 1;
            }

            resolutionDropdown.RefreshShownValue();
        }
    }


    public void OptionPopupOn() => panel.SetActive(true);
    public void OptionPopupOff()
    {
        panel.SetActive(false);
        Time.timeScale = 1.0f;

        if (UIManager.instance != null)
        {
            UIManager.instance.CursorChange(CursorLockMode.Locked);
        }
    }

    // 해상도 변경
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        SaveResolutionSettings(resolutionIndex); // 해상도 변경 시 저장
    }

    // BGM 볼륨 조절
    public void SetBgmVolume(float volume)
    {
        MasterAudio.PlaylistMasterVolume = volume;
        SaveVolumeSettings(); // 볼륨 설정 저장
    }

    // SFX 볼륨 조절
    public void SetSfxVolume(float volume)
    {
        MasterAudio.MasterVolumeLevel = volume;
        SaveVolumeSettings(); // 볼륨 설정 저장
    }
}

