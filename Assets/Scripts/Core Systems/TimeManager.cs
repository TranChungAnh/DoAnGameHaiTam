using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Settings")]
    public DayNightSettings settings;

    [Header("UI References")]
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;
    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public int Day { get; private set; }
    public int Month { get; private set; } = 1;
    public int Year { get; private set; } = 2025;
    public string Season { get; private set; } = "Spring";

    public delegate void MinuteEvent(int hour, int minute);
    public event MinuteEvent OnMinuteChanged;

    public delegate void HourEvent(int hour);
    public event HourEvent OnHourChanged;

    public delegate void DayEvent(int day, int month, int year);
    public event DayEvent OnDayPassed;

    public delegate void MonthEvent(int month, int year);
    public event MonthEvent OnMonthChanged;

    public delegate void YearEvent(int year);
    public event YearEvent OnYearChanged;

    private float timeAccumulator;

    //Thêm event Observer pattern
    public static event Action OnNightStarted;
    public static event Action OnDayStarted;
    private bool isNight = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Hour = settings != null ? settings.startHour : 6;
        Minute = settings != null ? settings.startMinute : 0;
        Day = 1;
    }

    private void Update()
    {
        // 1 ngày game = settings.dayLengthInMinutes phút thật
        // => mỗi phút game = (dayLengthInMinutes / (24*60)) phút thật
        timeAccumulator += Time.deltaTime;
        float secondsPerGameMinute = (settings.dayLengthInMinutes * 60f) / (24f * 60f);

        if (timeAccumulator >= secondsPerGameMinute)
        {
            timeAccumulator -= secondsPerGameMinute;
            AddMinutes(1);
        }

        if (dateText != null) dateText.text = $"{Day:D2}/{Month:D2}/{Year}";
        if (timeText != null) timeText.text = $"{Hour:D2}:{Minute:D2}";
    }

    public void AddMinutes(int mins)
    {
        Minute += mins;
        while (Minute >= 60)
        {
            Minute -= 60;
            Hour++;
            OnMinuteChanged?.Invoke(Hour, Minute);
            OnHourChanged?.Invoke(Hour);
            if (Hour == 22 && Minute == 0)
            {
                isNight = true;
                Debug.Log("🌙 Night started");
                OnNightStarted?.Invoke();
            }
            if (Hour == 6 && isNight)
            {
                isNight = false;
                Debug.Log("☀️ Day started");
                OnDayStarted?.Invoke(); // 🔹 Báo trời sáng
            }
            if (Hour >= 24)
            {
                Hour = 0;
                Day++;
                OnDayPassed?.Invoke(Day, Month, Year);
                if (SoilManager.Instance != null)
                {
                    SoilManager.Instance.NewDay();
                }
                if (Day > 30) // giả định 30 ngày/tháng
                {
                    Day = 1;
                    Month++;
                    OnMonthChanged?.Invoke(Month, Year);

                    if (Month > 12)
                    {
                        Month = 1;
                        Year++;
                        OnYearChanged?.Invoke(Year);
                    }
                }
            }
        }

        OnMinuteChanged?.Invoke(Hour, Minute);
    }

   
   
    public string GetTimeString() => $"{Hour:D2}:{Minute:D2}";
    public int GetAbsoluteDay() => (Year * 360) + (Month - 1) * 30 + (Day - 1);
}
