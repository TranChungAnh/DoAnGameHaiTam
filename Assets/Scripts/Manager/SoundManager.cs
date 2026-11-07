using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    public AudioSource voiceSound;
    public AudioSource diTrenCat;
    public AudioSource buocChanTrenGo;
    public AudioSource quai1Attack;
    public AudioSource tiengQuaiBiDanhTrung;
    public AudioSource nhatCoVat;
    public AudioSource audioClipNhatTaiLieu;
    public AudioSource audioClipTiengChiaKhoa;
    public AudioSource audioClipNhieu;
    public AudioSource anNutMoPhongThiNghiem;
    public AudioSource bgSound;
    public AudioSource roarSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if (soundToPlay != null)
        {
            soundToPlay.Stop(); // Dừng tiếng trước đó nếu đang phát dở
            soundToPlay.Play(); // Phát lại ngay lập tức
        }
    }

    public void PlayVoiceSound(AudioClip clip)
    {
        if (voiceSound == null || clip == null)
            return;

        voiceSound.clip = clip;

        if (!voiceSound.isPlaying)
        {
            voiceSound.Play();
        }
        else
        {
            voiceSound.Stop();
            voiceSound.Play();
        }
    }
}