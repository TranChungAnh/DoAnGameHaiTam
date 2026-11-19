
using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour
{
    public static AudioVolumeManager Instance { get; private set; }

    [Header("Slider")]
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Volume")]
    [Range(0f, 1f)] public float soundVolume = 0.5f;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 0.5f;

    private const string SoundKey = "SoundVolume";
    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        soundVolume = PlayerPrefs.GetFloat(SoundKey, 1f);
        musicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
        sfxVolume = PlayerPrefs.GetFloat(SFXKey, 1f);

        if (soundSlider != null)
        {
            soundSlider.minValue = 0f;
            soundSlider.maxValue = 1f;
            soundSlider.SetValueWithoutNotify(soundVolume);
            soundSlider.onValueChanged.AddListener(SetSoundVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.SetValueWithoutNotify(musicVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.SetValueWithoutNotify(sfxVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private void Start()
    {
        ApplyVolume();
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        PlayerPrefs.SetFloat(SoundKey, value);
        ApplyVolume();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat(MusicKey, value);
        ApplyVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat(SFXKey, value);
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (SoundManager.Instance == null)
            return;

        if (SoundManager.Instance.bgSound != null)
            SoundManager.Instance.bgSound.volume = musicVolume;

        if (SoundManager.Instance.sfxSound != null)
            SoundManager.Instance.sfxSound.volume = soundVolume;

        if (SoundManager.Instance.weaponSound != null)
            SoundManager.Instance.weaponSound.volume = sfxVolume;
    }
}
