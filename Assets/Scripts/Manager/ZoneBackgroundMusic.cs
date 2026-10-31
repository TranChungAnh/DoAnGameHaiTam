using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZoneBackgroundMusic : MonoBehaviour
{
    [Header("Cấu hình Âm thanh Vùng")]
    [Tooltip("AudioClip nhạc nền riêng cho vùng này")]
    public AudioClip regionBGM;

    [Tooltip("Danh sách các âm thanh phụ có thể phát cùng lúc (Ví dụ: tiếng gió, tiếng suối chảy...)")]
    public List<AudioClip> ambientSounds = new List<AudioClip>();

    [Header("Cài đặt Trigger")]
    [Tooltip("Chỉ cho phép kích hoạt 1 lần duy nhất khi player bước vào?")]
    [SerializeField] private bool triggerOnce = false;

    private bool hasTriggered = false;
    private List<AudioSource> spawnedAmbientSources = new List<AudioSource>();

    private void Awake()
    {
        // Đảm bảo Collider2D của vùng này luôn là Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng đi vào có phải là Player không
        if (collision.CompareTag("Player"))
        {
            if (triggerOnce && hasTriggered) return;

            // 1. Xử lý đổi nhạc nền (Đảm bảo SoundManager có một AudioSource riêng tên là bgSound hoặc dùng tạm voiceSound)
            if (regionBGM != null && SoundManager.Instance != null)
            {
                // LƯU Ý: Bạn nên vào SoundManager khai báo: public AudioSource bgSound; để chuyên dùng phát nhạc nền
                if (SoundManager.Instance.bgSound != null)
                {
                    AudioSource bgSource = SoundManager.Instance.bgSound; // Hoặc đổi thành bgSound nếu bạn đã tạo

                    if (bgSource.clip != regionBGM)
                    {
                        bgSource.clip = regionBGM;
                        bgSource.loop = true;
                        bgSource.Play();
                        Debug.Log($"🎵 Đã chuyển nhạc nền sang: {regionBGM.name}");
                    }
                }
            }

            // 2. Xử lý phát âm thanh môi trường (Được tách độc lập, không bị phụ thuộc vào việc có regionBGM hay không)
            if (ambientSounds != null && ambientSounds.Count > 0)
            {
                foreach (var clip in ambientSounds)
                {
                    if (clip != null)
                    {
                        // Tạo AudioSource phụ tạm thời gắn vào object này để phát chồng nhiều sound cùng lúc
                        AudioSource ambientSource = gameObject.AddComponent<AudioSource>();
                        ambientSource.clip = clip;
                        ambientSource.loop = true;
                        ambientSource.playOnAwake = false;
                        ambientSource.Play();

                        spawnedAmbientSources.Add(ambientSource);
                    }
                }
                Debug.Log($"🔊 Đã kích hoạt {ambientSounds.Count} âm thanh môi trường cùng lúc.");
            }

            hasTriggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (spawnedAmbientSources.Count > 0)
            {
                foreach (var source in spawnedAmbientSources)
                {
                    if (source != null) Destroy(source);
                }
                spawnedAmbientSources.Clear();
                Debug.Log("🔇 Đã dừng các âm thanh môi trường của vùng.");
            }
        }
    }
}