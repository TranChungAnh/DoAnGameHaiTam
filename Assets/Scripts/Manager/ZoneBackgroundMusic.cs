
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ZoneBackgroundMusic : MonoBehaviour
{
    public enum SoundType
    {
        BGSound,
        SFXSound
    }

    [Header("Cấu hình âm thanh")]
    [SerializeField] private SoundType soundType = SoundType.BGSound;
    [SerializeField] private AudioClip regionAudio;

    [Header("Cài đặt Trigger")]
    [SerializeField] private bool triggerOnce = false;

    private bool hasTriggered = false;
    private BoxCollider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<BoxCollider2D>();
        zoneCollider.isTrigger = true;

        // Bỏ qua Raycast
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void Start()
    {
        Collider2D[] players = Physics2D.OverlapBoxAll(
            zoneCollider.bounds.center,
            zoneCollider.bounds.size,
            zoneCollider.transform.eulerAngles.z
        );

        foreach (Collider2D player in players)
        {
            if (player.CompareTag("Player"))
            {
                PlayRegionAudio();
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayRegionAudio();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (SoundManager.Instance == null)
            return;

        switch (soundType)
        {
            case SoundType.BGSound:
                if (SoundManager.Instance.bgSound != null &&
                    SoundManager.Instance.bgSound.clip == regionAudio)
                {
                    SoundManager.Instance.bgSound.Stop();
                    SoundManager.Instance.bgSound.clip = null;
                }
                break;

            case SoundType.SFXSound:
                if (SoundManager.Instance.sfxSound != null)
                {
                    SoundManager.Instance.sfxSound.Stop();
                }
                break;
        }
    }

    private void PlayRegionAudio()
    {
        if (triggerOnce && hasTriggered)
            return;

        if (regionAudio == null)
        {
            Debug.LogWarning("Chưa chọn AudioClip: " + gameObject.name, this);
            return;
        }

        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy SoundManager!", this);
            return;
        }

        switch (soundType)
        {
            case SoundType.BGSound:
                SoundManager.Instance.PlayBackgroundMusic(regionAudio);
                break;

            case SoundType.SFXSound:
                SoundManager.Instance.PlaySound(regionAudio);
                break;
        }

        hasTriggered = true;
    }
}
