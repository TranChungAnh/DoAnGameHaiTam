using UnityEngine;

[CreateAssetMenu(fileName = "DayNightSettings", menuName = "FarmGame/DayNightSettings")]
public class DayNightSettings : ScriptableObject
{
    [Header("Time Settings")]
    [Tooltip("Thời gian thật (phút) tương ứng 1 ngày trong game")]
    public float dayLengthInMinutes = 10f;
    public int startHour = 6;
    public int startMinute = 0;

    [Header("Lighting Colors")]
    public Color dawnColor = new(1f, 0.5f, 0.3f);   // 5-7h
    public Color morningColor = new(1f, 0.85f, 0.6f);  // 7-10h
    public Color noonColor = Color.white;           // 10-15h
    public Color afternoonColor = new(1f, 0.8f, 0.5f);   // 15-18h
    public Color sunsetColor = new(1f, 0.4f, 0.2f);   // 18-20h
    public Color nightColor = new(0.1f, 0.1f, 0.25f);// 20-24h
    public Color midnightColor = new(0.05f, 0.05f, 0.15f);// 0-5h
}
