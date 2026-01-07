using UnityEditor.EditorTools;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; private set; }

    [Header("Multipliers")]
    [Tooltip("Hệ số bốc hơi khi trời nắng")]
    public float evaporationSunny = 0.0007f;
    [Tooltip("Hệ số bốc hơi khi trời mưa (thường = 0)")]
    public float evaporationRainy = 0.0f;
    [Tooltip("Lượng nước tăng/giây khi mưa")]
    public float rainMoistureGainPerSec = 0.02f;

    [Header("Weather VFX")]
    public ParticleSystem rainVFX;   // hiệu ứng mưa
    public ParticleSystem snowVFX;   // hiệu ứng tuyết

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
   
    }

    private void OnDisable()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= OnWeatherChanged;

        if (SeasonManager.Instance != null)
            SeasonManager.Instance.OnSeasonChanged -= OnSeasonChanged;

        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= OnDayPassed;
    }
    private void Start()
    {
        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged += OnWeatherChanged;

        if (SeasonManager.Instance != null)
            SeasonManager.Instance.OnSeasonChanged += OnSeasonChanged;

        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed += OnDayPassed;
    }

    private void Update()
    {
        if (WeatherManager.Instance == null || SoilManager.Instance == null) return;

        float delta = Time.deltaTime;

        if (WeatherManager.Instance.IsRaining())
        {
            foreach (var tile in SoilManager.Instance.GetAllSoilTiles())
            {
                if (tile.data.isTilled) // chỉ ảnh hưởng ô đã cày
                {
                    tile.data.moisture = Mathf.Clamp01(tile.data.moisture + delta * rainMoistureGainPerSec);
                    tile.data.isWatered = tile.data.moisture > 0f;
                }
            }
        }
        else
        {
            float evap = WeatherManager.Instance.currentWeather == WeatherType.Sunny ? evaporationSunny : evaporationRainy;

            foreach (var tile in SoilManager.Instance.GetAllSoilTiles())
            {
                if (tile.data.isTilled && tile.data.moisture > 0f)
                {
                    tile.data.moisture = Mathf.Max(0f, tile.data.moisture - delta * evap);
                    tile.data.isWatered = tile.data.moisture > 0f;
                }
            }
        }
    }


    private void OnWeatherChanged(WeatherType wt)
    {
        // Tắt hết VFX trước
        if (rainVFX != null) rainVFX.Stop();
        if (snowVFX != null) snowVFX.Stop();

        // Bật theo loại thời tiết
        switch (wt)
        {
            case WeatherType.Rainy:
                if (rainVFX != null) rainVFX.Play();
                break;

            case WeatherType.Snowy:
                if (snowVFX != null) snowVFX.Play();
                Debug.Log("It's snowing!");  // test
                break;

            default:
                // Sunny, Cloudy, Windy... thì tắt hết
                break;
        }
    }

    private void OnSeasonChanged(Season s)
    {
        // Điều chỉnh ánh sáng nền theo mùa
        if (LightingManager.Instance != null)
        {
            switch (s)
            {
                case Season.Winter: LightingManager.Instance.sunLight.intensity = 0.9f; break;
                case Season.Summer: LightingManager.Instance.sunLight.intensity = 1.2f; break;
                default: LightingManager.Instance.sunLight.intensity = 1.0f; break;
            }
        }
    }

    private void OnDayPassed(int day, int month, int year)
    {
        // UpdateMoisture hệ sinh thái theo ngày
        CropManager.Instance?.TickGrowthDaily();
        AnimalManager.Instance?.TickDaily();
        ResourceSpawner.Instance?.TryDailyRespawn();
    }
}
