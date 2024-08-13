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
    private AudioSource effectAudioSource;
    private AudioSource bgmAudioSource;
    private float masterVolume = 1f;
    private float savedBgmVolume;
    private float savedEffectVolume;

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
            effectAudioSource = GetComponent<AudioSource>();
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.volume = 0.2f;
            bgmAudioSource.loop = true;

            InitializeVolumeBars();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void InitializeVolumeBars()
    {
        instance.FindVolumeBars();
        LoadVolumeSettings();

        if (instance.masterVolumeBar != null)
        {
            instance.masterVolumeBar.onValueChanged.AddListener(SetMasterVolume);
            instance.masterVolumeBar.value = instance.masterVolume;
        }

        if (instance.bgmVolumeBar != null)
        {
            instance.bgmVolumeBar.onValueChanged.AddListener((value) => SetVolume(SoundType.Bgm, value));
            instance.bgmVolumeBar.value = instance.savedBgmVolume;
        }

        if (instance.effectVolumeBar != null)
        {
            instance.effectVolumeBar.onValueChanged.AddListener((value) => SetVolume(SoundType.Effect, value));
            instance.effectVolumeBar.value = instance.savedEffectVolume;
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
    }

    public static void SetVolume(SoundType sound, float volume)
    {
        volume = Mathf.Clamp01(volume);
        instance.volumeLevels[sound] = volume;

        if (sound == SoundType.Bgm)
            instance.savedBgmVolume = volume;
        else if (sound == SoundType.Effect)
            instance.savedEffectVolume = volume;

        UpdateVolumes();
    }


    private static void UpdateVolumes()
    {
        instance.bgmAudioSource.volume = instance.masterVolume * instance.volumeLevels[SoundType.Bgm];
        instance.effectAudioSource.volume = instance.masterVolume * instance.volumeLevels[SoundType.Effect];
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
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            instance.masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        }
        if (PlayerPrefs.HasKey("BgmVolume"))
        {
            instance.volumeLevels[SoundType.Bgm] = PlayerPrefs.GetFloat("BgmVolume");
        }
        if (PlayerPrefs.HasKey("EffectVolume"))
        {
            instance.volumeLevels[SoundType.Effect] = PlayerPrefs.GetFloat("EffectVolume");
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
