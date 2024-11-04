using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public enum SoundType
{
    Bgm,
    Effect
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmList;
    [SerializeField] private AudioClip[] effectList;
    private static SoundManager instance;
    public AudioSource effectAudioSource;
    public AudioSource bgmAudioSource;
    private float masterVolume = 1f;
    private bool sceneLoaded = false;
    
    [Header("UI Elements")]
    [SerializeField] private Scrollbar masterVolumeBar;
    [SerializeField] private Scrollbar bgmVolumeBar;
    [SerializeField] private Scrollbar effectVolumeBar;

    private Dictionary<SoundType, float> volumeLevels = new Dictionary<SoundType, float>()
    {
        { SoundType.Bgm, 0.2f },
        { SoundType.Effect, 1f }
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            GameObject bgmObject = GameObject.Find("BGM");
            if (bgmObject != null)
            {
                bgmAudioSource = bgmObject.GetComponent<AudioSource>();
                if (bgmAudioSource == null)
                {
                    bgmAudioSource = bgmObject.AddComponent<AudioSource>();
                }
                bgmAudioSource.loop = true;
            }
            else
            {
                Debug.LogError("BGM GameObject not found.");
                return;
            }

            GameObject effectObject = GameObject.Find("GameSceneMgr");
            if (effectObject != null)
            {
                effectAudioSource = effectObject.GetComponent<AudioSource>();
                if (effectAudioSource == null)
                {
                    effectAudioSource = effectObject.AddComponent<AudioSource>();
                }
            }
            else if(SceneManager.GetActiveScene().name == "StartScene")
            {
                Debug.Log("No Effect in StartScene");
            }
            else
            {
                Debug.LogWarning("Effect GameObject not found");
            }

            StartCoroutine(FindEffectSource());

            FindVolumeBars();
            LoadVolumeSettings();
            InitializeVolumeBars();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FindEffectSource()
    {
        while (true)
        {
            if (effectAudioSource == null)
            {
                GameObject effectObject = GameObject.Find("GameSceneMgr");
                if (effectObject != null)
                {
                    effectAudioSource = effectObject.GetComponent<AudioSource>();
                    if (effectAudioSource == null)
                    {
                        effectAudioSource = effectObject.AddComponent<AudioSource>();
                        yield return StartCoroutine(WaitForSceneChange());
                    }
                }
                else if (SceneManager.GetActiveScene().name == "StartScene" || SceneManager.GetActiveScene().name == "IntroScene")
                {
                    yield return StartCoroutine(WaitForSceneChange());
                }
                else
                {
                    Debug.LogWarning("Effect GameObject not found");
                }
            }
            else
            {
                yield return StartCoroutine(WaitForSceneChange());
            }
            yield return null;
        }
    }
    
    private IEnumerator WaitForSceneChange()
    {
        sceneLoaded = false;
        SceneManager.sceneLoaded += OnSceneLoaded;
        yield return new WaitUntil(() => sceneLoaded);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneLoaded = true;
    }

    public static void InitializeVolumeBars()
    {
        instance.FindVolumeBars();
        LoadVolumeSettings();

        if (instance.masterVolumeBar != null)
        {
            instance.masterVolumeBar.value = instance.masterVolume;
            instance.masterVolumeBar.onValueChanged.RemoveAllListeners();
            instance.masterVolumeBar.onValueChanged.AddListener(SetMasterVolume);
        }

        if (instance.bgmVolumeBar != null)
        {
            instance.bgmVolumeBar.value = instance.volumeLevels[SoundType.Bgm];
            instance.bgmVolumeBar.onValueChanged.RemoveAllListeners();
            instance.bgmVolumeBar.onValueChanged.AddListener((value) => SetVolume(SoundType.Bgm, value));
        }

        if (instance.effectVolumeBar != null && instance.effectAudioSource != null)
        {
            instance.effectVolumeBar.value = instance.volumeLevels[SoundType.Effect];
            instance.effectVolumeBar.onValueChanged.RemoveAllListeners();
            instance.effectVolumeBar.onValueChanged.AddListener((value) => SetVolume(SoundType.Effect, value));
        }
    }

    private void FindVolumeBars()
    {
        masterVolumeBar = GameObject.Find("MasterBar")?.GetComponent<Scrollbar>();
        bgmVolumeBar = GameObject.Find("BgmBar")?.GetComponent<Scrollbar>();
        effectVolumeBar = GameObject.Find("EffectBar")?.GetComponent<Scrollbar>();
    }

    public static void SetMasterVolume(float volume)
    {
        instance.masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
        SaveVolumeSettings();
    }

    public static void SetVolume(SoundType sound, float volume)
    {
        volume = Mathf.Clamp01(volume);
        instance.volumeLevels[sound] = volume;

        if (sound == SoundType.Bgm)
            instance.bgmAudioSource.volume = volume;
        else if (sound == SoundType.Effect)
            instance.effectAudioSource.volume = volume;

        UpdateVolumes();

        SaveVolumeSettings();
    }


    private static void UpdateVolumes()
    {
        if (instance.bgmAudioSource != null)
        {
            instance.bgmAudioSource.volume = instance.masterVolume * instance.volumeLevels[SoundType.Bgm];
        }

        if (instance.effectAudioSource != null)
        {
            instance.effectAudioSource.volume = instance.masterVolume * instance.volumeLevels[SoundType.Effect];
        }
    }


    public static void BGMPlayer(string clipName)
    {
        AudioClip bgmClip = instance.bgmList.FirstOrDefault(x => x.name.Equals(clipName));
        if (bgmClip != null && instance.bgmAudioSource.clip != bgmClip)
        {
            instance.bgmAudioSource.Stop();
            instance.bgmAudioSource.clip = bgmClip;
            instance.bgmAudioSource.Play();
        }
    }
    public static void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", instance.masterVolume);
        PlayerPrefs.SetFloat("BgmVolume", instance.volumeLevels[SoundType.Bgm]);
        PlayerPrefs.SetFloat("EffectVolume", instance.volumeLevels[SoundType.Effect]);
        PlayerPrefs.Save();
    }

    public static void LoadVolumeSettings()
    {
        instance.masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        instance.volumeLevels[SoundType.Bgm] = PlayerPrefs.GetFloat("BgmVolume", 0.2f);
        instance.volumeLevels[SoundType.Effect] = PlayerPrefs.GetFloat("EffectVolume", 1.0f);
        if (instance.masterVolumeBar != null)
        {
            instance.masterVolumeBar.value = instance.masterVolume;
        }

        if (instance.bgmVolumeBar != null)
        {
            instance.bgmVolumeBar.value = instance.volumeLevels[SoundType.Bgm];
            instance.bgmAudioSource.volume = instance.volumeLevels[SoundType.Bgm] * instance.masterVolume;
        }

        if (instance.effectVolumeBar != null && instance.effectAudioSource != null)
        {
            instance.effectVolumeBar.value = instance.volumeLevels[SoundType.Effect];
            instance.effectAudioSource.volume = instance.volumeLevels[SoundType.Effect] * instance.masterVolume;
        }
    }


    public static void PlayOneShot(string audioName)
    {
        var clip = instance.effectList.FirstOrDefault(x => x.name.Equals(audioName));
        if (clip != null)
        {
            instance.effectAudioSource.PlayOneShot(clip);
        }
    }

    public static void PlaySound(SoundType sound, int index, float volume = 1f)
    {
        AudioClip clip = null;
        float soundVolume = instance.volumeLevels[sound];

        switch (sound)
        {
            case SoundType.Bgm:
                if (index >= 0 && index < instance.bgmList.Length)
                {
                    clip = instance.bgmList[index];
                    instance.bgmAudioSource.clip = clip;
                    instance.bgmAudioSource.volume = volume * instance.masterVolume * soundVolume;
                    instance.bgmAudioSource.Play();
                }
                break;
            case SoundType.Effect:
                if (index >= 0 && index < instance.effectList.Length)
                {
                    clip = instance.effectList[index];
                    instance.effectAudioSource.PlayOneShot(clip, volume * instance.masterVolume * soundVolume);
                }
                break;
        }
    }
}