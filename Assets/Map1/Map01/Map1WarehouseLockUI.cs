using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Gan script len Canvas (Canvas phai ACTIVE); Panel o khoa co the de INACTIVE.
public class Map1WarehouseLockUI : MonoBehaviour
{
    [Header("Giao dien")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image keyImage;
    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button unlockButton;
    [SerializeField] private Button closeButton;

    private Map1WarehouseLock currentLock;

    private void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (unlockButton != null)
            unlockButton.onClick.AddListener(OnUnlockClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    private void Update()
    {
        if (panel != null && panel.activeSelf)
            Refresh();
    }

    public void Show(Map1WarehouseLock target)
    {
        if (target == null || panel == null) return;

        currentLock = target;

        if (keyImage != null)
        {
            keyImage.sprite = target.KeyIcon;
            keyImage.enabled = target.KeyIcon != null;
        }

        panel.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        bool hasKey = currentLock != null && currentLock.HasKey();
        bool canOpen = hasKey && currentLock != null &&
                       !currentLock.IsOpened && currentLock.IsPlayerInRange();

        if (keyCountText != null)
            keyCountText.text = hasKey ? "1/1" : "0/1";

        if (unlockButton != null)
            unlockButton.interactable = canOpen;

        if (messageText != null)
            messageText.text = hasKey
                ? "Đã có chìa khóa. Bạn có thể mở cửa."
                : "Cần 1 chìa khóa phòng lưu trữ.";
    }

    private void OnUnlockClicked()
    {
        if (currentLock != null && currentLock.TryUnlock())
            Close();
        else
            Refresh();
    }

    public void CloseIfCurrent(Map1WarehouseLock target)
    {
        if (currentLock == target)
            Close();
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        currentLock = null;
    }

    private void OnDestroy()
    {
        if (unlockButton != null)
            unlockButton.onClick.RemoveListener(OnUnlockClicked);

        if (closeButton != null)
            closeButton.onClick.RemoveListener(Close);
    }
}
