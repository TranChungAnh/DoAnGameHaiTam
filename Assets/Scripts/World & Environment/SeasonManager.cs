using UnityEngine;

public enum Season { Spring, Summer, Autumn, Winter }

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; private set; }

    [Header("Chọn mùa cho từng tháng (1-12)")]
    public Season[] monthSeasons = new Season[12];   // chỉnh ngoài Inspector

    public Season CurrentSeason { get; private set; } = Season.Spring;

    public delegate void SeasonEvent(Season newSeason);
    public event SeasonEvent OnSeasonChanged;

    private int cachedMonth = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (TimeManager.Instance == null) return;

        int month = TimeManager.Instance.Month;
        if (month != cachedMonth)
        {
            cachedMonth = month;
            UpdateSeasonByMonth(month);
        }
    }

    private void UpdateSeasonByMonth(int month)
    {
        // Lấy mùa từ mảng (month - 1 vì mảng bắt đầu từ 0)
        Season newSeason = monthSeasons[Mathf.Clamp(month - 1, 0, 11)];

        if (newSeason != CurrentSeason)
        {
            CurrentSeason = newSeason;
            OnSeasonChanged?.Invoke(CurrentSeason);
            // Debug.Log($"Season → {CurrentSeason}");
        }
    }
}
