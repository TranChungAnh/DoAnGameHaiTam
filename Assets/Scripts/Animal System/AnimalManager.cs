using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }
    public List<Animal> animals = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterAnimal(Animal animal) => animals.Add(animal);
    public void UnregisterAnimal(Animal animal) => animals.Remove(animal);

    public IEnumerable<Animal> GetAllAnimals() => animals;

    // =====================
    // 🔽 Code mở rộng thêm
    // =====================
    [Header("Animal Settings")]
    public bool affectByWeather = true;
    public bool affectBySeason = true;

    void OnEnable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed += OnDayPassed;

        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged += OnWeatherChanged;

        if (SeasonManager.Instance != null)
            SeasonManager.Instance.OnSeasonChanged += OnSeasonChanged;
    }

    void OnDisable()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnDayPassed -= OnDayPassed;

        if (WeatherManager.Instance != null)
            WeatherManager.Instance.OnWeatherChanged -= OnWeatherChanged;

        if (SeasonManager.Instance != null)
            SeasonManager.Instance.OnSeasonChanged -= OnSeasonChanged;
    }

    // ⚡ thêm cho EnvironmentManager gọi (không bắt buộc Animal phải có method)
    public void TickDaily()
    {
        int d = TimeManager.Instance != null ? TimeManager.Instance.Day : 0;
        int m = TimeManager.Instance != null ? TimeManager.Instance.Month : 0;
        int y = TimeManager.Instance != null ? TimeManager.Instance.Year : 0;

        foreach (var animal in animals)
        {
            if (animal == null) continue;
            var recv = animal as IAnimalEvents; // chỉ gọi nếu Animal implement
            recv?.OnNewDay(d, m, y);
        }
    }

    // TimeManager.OnDayPassed có chữ ký (int day, int month, int year)
    private void OnDayPassed(int day, int month, int year)
    {
        foreach (var animal in animals)
        {
            if (animal == null) continue;
            var recv = animal as IAnimalEvents;
            recv?.OnNewDay(day, month, year);
        }
    }

    private void OnWeatherChanged(WeatherType newWeather)
    {
        if (!affectByWeather) return;

        foreach (var animal in animals)
        {
            if (animal == null) continue;
            var recv = animal as IAnimalEvents;
            recv?.OnWeatherChanged(newWeather);
        }
    }

    private void OnSeasonChanged(Season newSeason)
    {
        if (!affectBySeason) return;

        foreach (var animal in animals)
        {
            if (animal == null) continue;
            var recv = animal as IAnimalEvents;
            recv?.OnSeasonChanged(newSeason);
        }
    }
    public interface IAnimalEvents
    {
        void OnNewDay(int day, int month, int year);
        void OnWeatherChanged(WeatherType newWeather);
        void OnSeasonChanged(Season newSeason);
    }

}
