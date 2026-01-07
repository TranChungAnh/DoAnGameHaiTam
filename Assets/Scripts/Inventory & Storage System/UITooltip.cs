using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    public static UITooltip Instance;

    public Text tooltipText;
    public RectTransform backgroundRect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gameObject.SetActive(false);
    }

    public static void Show(string text, Vector3 position)
    {
        if (Instance == null) return;

        Instance.gameObject.SetActive(true);
        Instance.tooltipText.text = text;

        // Move tooltip đến đúng vị trí
        Instance.transform.position = position;
    }

    public static void Hide()
    {
        if (Instance == null) return;
        Instance.gameObject.SetActive(false);
    }
}
