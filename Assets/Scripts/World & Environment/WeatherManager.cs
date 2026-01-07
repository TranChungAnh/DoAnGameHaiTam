using GameEnums;
using UnityEngine;

public enum WeatherType { Sunny, Cloudy, Rainy, Snowy, Windy }

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }

    [Header("Config")]
    public WeatherType currentWeather = WeatherType.Sunny;
    [Tooltip("Thời gian tối thiểu 1 kiểu thời tiết (giây game)")]
    public float minDuration = 45f;
    [Tooltip("Thời gian tối đa 1 kiểu thời tiết (giây game)")]
    public float maxDuration = 120f;

    [Header("Season Weights (0..1)")]
    [Range(0f, 1f)] public float springRain = 0.5f;
    [Range(0f, 1f)] public float summerRain = 0.3f;
    [Range(0f, 1f)] public float autumnRain = 0.4f;
    [Range(0f, 1f)] public float winterSnow = 0.6f;

    private float timer;
    private float currentDuration;

    public delegate void WeatherEvent(WeatherType wt);
    public event WeatherEvent OnWeatherChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        PickNextDuration();
    }
    private void Start()
    {
        OnWeatherChanged?.Invoke(currentWeather);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= currentDuration)
        {
            timer = 0f;
            RandomizeWeather();
            PickNextDuration();
        }

        if (IsRaining())
        {
            float delta = Time.deltaTime;

            foreach (var tile in SoilManager.Instance.GetAllSoilTiles())
            {
                if (tile.data.isTilled) // chỉ ô đã cày
                {
                    // tăng moisture
                    tile.data.moisture = Mathf.Clamp01(tile.data.moisture + delta * 0.2f);
                    tile.data.isWatered = tile.data.moisture > 0f;

                    if (!tile.data.hasRainTimer)
                    {
                        tile.data.hasRainTimer = true;
                        tile.data.rainTimer = 0f;
                    }

                    if (tile.data.hasRainTimer)
                    {
                        tile.data.rainTimer += delta;
                        if (tile.data.rainTimer >= 5f) // 5 giây sau mới đổi tile
                        {
                            SoilManager.Instance.SetTileVisual(tile.data.position, true, true);
                            tile.data.hasRainTimer = false; // reset timer
                        }
                    }
                }
            }
        }
        else
        {
            // Nếu trời không mưa, reset các timer
            foreach (var tile in SoilManager.Instance.GetAllSoilTiles())
            {
                tile.data.hasRainTimer = false;
                tile.data.rainTimer = 0f;
            }
        }
    }


    private void PickNextDuration()
    {
        currentDuration = Random.Range(minDuration, maxDuration);
    }

    public void RandomizeWeather()
    {
        Season s = SeasonManager.Instance != null ? SeasonManager.Instance.CurrentSeason : Season.Spring;

        // Trọng số đơn giản theo mùa
        float pRain = 0.2f;
        float pSnow = 0.05f;

        switch (s)
        {
            case Season.Spring: pRain = Mathf.Clamp01(springRain); pSnow = 0.0f; break;
            case Season.Summer: pRain = Mathf.Clamp01(summerRain); pSnow = 0.0f; break;
            case Season.Autumn: pRain = Mathf.Clamp01(autumnRain); pSnow = 0.05f; break;
            case Season.Winter: pRain = 0.1f; pSnow = Mathf.Clamp01(winterSnow); break;
        }

        float r = Random.value;
        WeatherType next;
        if (s == Season.Winter && r < pSnow) next = WeatherType.Snowy;
        else if (r < pRain) next = WeatherType.Rainy;
        else if (r < pRain + 0.15f) next = WeatherType.Windy;
        else if (r < pRain + 0.15f + 0.25f) next = WeatherType.Cloudy;
        else next = WeatherType.Sunny;

        SetWeather(next);
    }

    public void SetWeather(WeatherType wt)
    {
        if (currentWeather == wt) return;
        currentWeather = wt;

        if (OnWeatherChanged == null)
        {
            Debug.LogWarning("⚠️ OnWeatherChanged hiện đang NULL (không có listener nào).");
        }
        else
        {
            Debug.Log("✅ Có listener đăng ký OnWeatherChanged.");
            OnWeatherChanged.Invoke(currentWeather);
        }

        Debug.Log($"Weather → {currentWeather}");
    }


    public bool IsRaining() => currentWeather == WeatherType.Rainy;
    public bool IsSunny() => currentWeather == WeatherType.Sunny;

    public bool IsSnowing() => currentWeather == WeatherType.Snowy;
    public bool IsWindy() => currentWeather == WeatherType.Windy;
}
