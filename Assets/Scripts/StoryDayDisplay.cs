using TMPro;
using UnityEngine;

public class StoryDayDisplay : MonoBehaviour
{
    [SerializeField, Min(1)] private int mapDay = 1;
    [SerializeField] private TMP_Text dayText;

    public static int HighestDay { get; private set; } = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetForNewGame()
    {
        HighestDay = 1;
    }

    private void OnEnable()
    {
        RefreshDay();
    }

    private void RefreshDay()
    {
        HighestDay = Mathf.Max(HighestDay, mapDay);

        if (dayText != null)
            dayText.text = $"Day {HighestDay}";
    }

    public static void RestoreFromSave(int savedDay)
    {
        HighestDay = Mathf.Max(1, savedDay);

        StoryDayDisplay display = FindAnyObjectByType<StoryDayDisplay>();
        if (display != null)
            display.RefreshDay();
    }
}