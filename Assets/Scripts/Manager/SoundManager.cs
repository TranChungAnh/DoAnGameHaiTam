
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Nguồn phát âm thanh")]
    public AudioSource bgSound;
    public AudioSource sfxSound;
    public AudioSource weaponSound; // Âm thanh vung kiếm riêng

    [Header("Âm thanh")]
    public AudioClip diTrenCat;
    public AudioClip buocChanTrenGo;
    public AudioClip quai1Attack;
    public AudioClip tiengQuaiBiDanhTrung;
    public AudioClip nhatCoVat;
    public AudioClip audioClipNhatTaiLieu;
    public AudioClip audioClipTiengChiaKhoa;
    public AudioClip audioClipNhieu;
    public AudioClip roarSound;
    public AudioClip vungkiem;

    public AudioClip M2_nutbam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (weaponSound != null)
        {
            weaponSound.playOnAwake = false;
            weaponSound.loop = false;
        }
        ApplyVolume();
    }
    public void PlaySFX(AudioClip clip)
    {
        if (weaponSound != null && clip != null)
        {
            weaponSound.PlayOneShot(clip);
        }
    }
    public void ApplyVolume()
    {
        if (AudioVolumeManager.Instance == null)
            return;

        if (bgSound != null)
            bgSound.volume = AudioVolumeManager.Instance.musicVolume;

        if (sfxSound != null)
            sfxSound.volume = AudioVolumeManager.Instance.soundVolume;

        if (weaponSound != null)
            weaponSound.volume = AudioVolumeManager.Instance.sfxVolume;
    }
    // Phát nhạc nền
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (bgSound == null || clip == null)
            return;

        if (bgSound.clip == clip && bgSound.isPlaying)
            return;

        bgSound.Stop();
        bgSound.clip = clip;
        bgSound.loop = true;
        bgSound.Play();
    }

    // Phát âm thanh ngắn
    public void PlaySound(AudioClip clip)
    {
        if (sfxSound != null && clip != null)
        {
            sfxSound.PlayOneShot(clip);
        }
    }

    // Phát âm thanh vung kiếm
    public void PlayWeaponSound()
    {
        if (weaponSound != null && vungkiem != null)
        {
            weaponSound.Stop();
            weaponSound.clip = vungkiem;
            weaponSound.Play();
        }
    }

    // Dừng âm thanh vung kiếm
    public void StopWeaponSound()
    {
        if (weaponSound != null)
        {
            weaponSound.Stop();
        }
    }
}
