using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance { get; private set; }
    public DayNightSettings settings;
    public Light2D sunLight;
    [Range(0.1f, 5f)] public float colorLerpSpeed = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (TimeManager.Instance == null || settings == null || sunLight == null) return;
        UpdateLighting();
    }

    private void UpdateLighting()
    {
        int hour = TimeManager.Instance.Hour;
        int minute = TimeManager.Instance.Minute;

        Color targetColor;
        if (hour >= 5 && hour < 7)
            targetColor = Color.Lerp(settings.nightColor, settings.dawnColor, (hour - 5 + minute / 60f) / 2f);
        else if (hour >= 7 && hour < 10)
            targetColor = Color.Lerp(settings.dawnColor, settings.morningColor, (hour - 7 + minute / 60f) / 3f);
        else if (hour >= 10 && hour < 15)
            targetColor = Color.Lerp(settings.morningColor, settings.noonColor, (hour - 10 + minute / 60f) / 5f);
        else if (hour >= 15 && hour < 18)
            targetColor = Color.Lerp(settings.noonColor, settings.afternoonColor, (hour - 15 + minute / 60f) / 3f);
        else if (hour >= 18 && hour < 20)
            targetColor = Color.Lerp(settings.afternoonColor, settings.sunsetColor, (hour - 18 + minute / 60f) / 2f);
        else if (hour >= 20 && hour < 24)
            targetColor = Color.Lerp(settings.sunsetColor, settings.midnightColor, (hour - 20 + minute / 60f) / 4f);
        else
            targetColor = Color.Lerp(settings.midnightColor, settings.nightColor, (hour + minute / 60f) / 5f);

        sunLight.color = Color.Lerp(sunLight.color, targetColor, Time.deltaTime * colorLerpSpeed);

        float timePercent = (hour + minute / 60f) / 24f;
        sunLight.transform.rotation = Quaternion.Euler(new Vector3(timePercent * 360f - 90f, 170, 0));
    }
}
