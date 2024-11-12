using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioSource backgroundMusic;
    public AudioSource tenseBGM;
    public AudioSource[] sfxSources;

    private RadialProgressbars radialProgressbars;

    private void Awake()
    {
        if (backgroundMusic == null)
        {
            backgroundMusic = GetComponent<AudioSource>();
            if (backgroundMusic == null)
            {
                Debug.LogError("No AudioSource component found on this object.");
                return;
            }
        }

        // 배경음악 재생
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.volume = 1.0f;
            backgroundMusic.Play();
            Debug.Log("Background music started playing.");
        }
        else
        {
            Debug.Log("Background music AudioSource is already playing.");
        }

        // 초기 설정값 로드 및 적용
        float initialMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 100.0f);
        float initialMusicVolume = PlayerPrefs.GetFloat("BGMVolume", 100.0f);
        float initialSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 100.0f);

        // 초기 값을 실제로 설정
        SetMasterVolume(initialMasterVolume);
        SetMusicVolume(initialMusicVolume);
        SetSfxVolume(initialSfxVolume);

        // RadialProgressbars 참조 가져오기
        radialProgressbars = FindObjectOfType<RadialProgressbars>();

        // 감염률 체크 주기적으로 호출
        StartCoroutine(CheckInfectionRateRoutine());
    }

    public void SetMasterVolume(float volume)
    {
        float normalizedVolume = volume; // 0~100 범위를 0~1로 변환
        backgroundMusic.volume = normalizedVolume;

        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = normalizedVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume;
        }
        else
        {
            Debug.LogError("Background music AudioSource is not assigned.");
        }
    }

    public void SetSfxVolume(float volume)
    {
        foreach (var sfxSource in sfxSources)
        {
            sfxSource.volume = volume;
        }
    }

    // 긴장감 있는 BGM으로 전환하는 함수 추가
    public void SwitchToTenseBGM()
    {
        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        if (!tenseBGM.isPlaying)
        {
            tenseBGM.Play();
            Debug.Log("Switched to tense background music.");
        }
    }

    // 일반 배경음악으로 돌아가는 함수 추가
    public void SwitchToNormalBGM()
    {
        if (tenseBGM.isPlaying)
        {
            tenseBGM.Stop();
        }

        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
            Debug.Log("Switched to normal background music.");
        }
    }

    // 감염률 체크 루틴 추가
    IEnumerator CheckInfectionRateRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5초마다 체크

            if (radialProgressbars != null)
            {
                float infectionRate = radialProgressbars.GetHospitalInfectionRate();
                Debug.Log($"Current Hospital Infection Rate: {infectionRate}%");

                if (infectionRate > 10f)
                {
                    SwitchToTenseBGM();
                }
                else
                {
                    SwitchToNormalBGM();
                }
            }
        }
    }
}
