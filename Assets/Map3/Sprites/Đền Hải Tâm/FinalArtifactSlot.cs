using UnityEngine;
using TMPro;

public class FinalArtifactSlot : MonoBehaviour, IInteractable
{
    [Header("Phải khớp Item Name trong dữ liệu vật phẩm")]
    [SerializeField] private string requiredItemName;
    [SerializeField] private string displayItemName;

    [Header("Cổ vật và ánh sáng đã đặt sẵn trên bệ")]
    [SerializeField] private GameObject artifactVisual;
    [SerializeField] private GameObject beamEffect;

    [Header("Bộ quản lý chung của 6 bệ")]
    [SerializeField] private FinalArtifactController controller;

    [Header("Thông báo")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text messageTitle;

    [Header("Ẩn nền ký ức khi hiện thông báo")]
    [SerializeField] private GameObject memoryBackground;

    private bool isPlaced;

    private void Awake()
    {
        if (artifactVisual != null)
            artifactVisual.SetActive(false);

        if (beamEffect != null)
            beamEffect.SetActive(false);
    }

    public void Interact()
    {
        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsTalking)
        {
            return;
        }

        string itemLabel = string.IsNullOrWhiteSpace(displayItemName)
            ? requiredItemName
            : displayItemName;

        // Trụ đã có cổ vật.
        if (isPlaced)
        {
            ShowMessage(
                "Đã đặt cổ vật",
                $"{itemLabel} đã được đặt trên trụ này rồi."
            );
            return;
        }

        if (string.IsNullOrWhiteSpace(requiredItemName))
        {
            Debug.LogError($"{name}: Chưa gán Required Item Name.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy InventoryManager.");
            return;
        }

        // Chưa có cổ vật trong hành trang.
        if (!InventoryManager.Instance.HasItem(requiredItemName))
        {
            ShowMessage(
                "Chưa có cổ vật",
                $"Bạn chưa có {itemLabel}. Hãy tìm cổ vật để đặt vào trụ này."
            );
            return;
        }

        // Kiểm tra bệ trước khi trừ vật phẩm.
        if (artifactVisual == null ||
            beamEffect == null ||
            controller == null)
        {
            Debug.LogError(
                $"{name}: Chưa gán Artifact Visual, Beam Effect hoặc Controller.");
            return;
        }

        if (!InventoryManager.Instance.RemoveItem(requiredItemName))
        {
            ShowMessage(
                "Không thể đặt",
                "Không lấy được cổ vật từ hành trang. Hãy thử lại."
            );
            return;
        }

        isPlaced = true;

        artifactVisual.SetActive(true);
        beamEffect.SetActive(true);

        ShowMessage(
            "Đặt thành công",
            $"{itemLabel} đã được đặt vào trụ."
        );

        UIInventory inventoryUI = FindObjectOfType<UIInventory>();

        if (inventoryUI != null &&
            inventoryUI.gameObject.activeInHierarchy)
        {
            inventoryUI.Refresh();
        }

        controller.RegisterPlacedSlot(this);
    }

    private void ShowMessage(string title, string message)
    {
        if (messagePanel == null || messageText == null)
        {
            Debug.LogWarning(
                $"{name}: Chưa gán Message Panel hoặc Message Text. " +
                $"Thông báo: {message}"
            );
            return;
        }

        if (messageTitle != null)
            messageTitle.text = title;

        messageText.text = message;
        messagePanel.SetActive(true);

        if (memoryBackground != null)
            memoryBackground.SetActive(false);

        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 2f);
    }

    private void HideMessage()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }
}