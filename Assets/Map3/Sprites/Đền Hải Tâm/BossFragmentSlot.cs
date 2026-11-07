using UnityEngine;
using TMPro;

public class BossFragmentSlot : MonoBehaviour, IInteractable
{
    [Header("Tên mảnh cần đặt - phải khớp Item Name")]
    [SerializeField] private string requiredItemName;
    [SerializeField] private string displayItemName;

    [Header("Phần tranh Boss sẽ hiện màu khi đặt đúng")]
    [SerializeField] private SpriteRenderer portraitPart;

    [Header("Thông báo khi chưa có mảnh hoặc chưa gán Dialog")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private TMP_Text successText;
    [SerializeField] private TMP_Text successTitle;

    [Header("Ánh sáng nối từ trụ đến bệ trung tâm")]
    [SerializeField] private GameObject beamEffect;

    [Header("Dialog tương ứng với trụ này")]
    [Tooltip("Kéo đối tượng Tranh1_DialogueZone hoặc vùng tương ứng vào đây.")]
    [SerializeField] private GameObject dialogueZone;

    [Header("Quản lý bệ trung tâm")]
    [SerializeField] private TempleCenterController centerController;

    [SerializeField] private GameObject memoryBackground;
    private bool isPlaced;

    private void Start()
    {
        if (beamEffect != null)
            beamEffect.SetActive(false);
    }

    public void Interact()
    {
        // Không đặt thêm mảnh khi đang xem Dialog.
        if (DialogueManager.Instance != null &&
            DialogueManager.Instance.IsTalking)
        {
            return;
        }

        if (isPlaced)
        {
            Debug.Log("Trụ này đã có mảnh rồi.");
            return;
        }

        if (string.IsNullOrEmpty(requiredItemName) ||
            portraitPart == null)
        {
            Debug.LogError(
                $"{gameObject.name}: chưa gán tên mảnh hoặc phần tranh Boss.");
            return;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("Không tìm thấy InventoryManager trong Scene.");
            return;
        }

        if (!InventoryManager.Instance.HasItem(requiredItemName))
        {
            ShowMessage(
                "Chưa có mảnh",
                $"Bạn chưa có {displayItemName}. Hãy đi tìm mảnh này."
            );
            return;
        }

        if (!InventoryManager.Instance.RemoveItem(requiredItemName))
        {
            Debug.LogError($"Không trừ được mảnh: {requiredItemName}");
            return;
        }

        isPlaced = true;

        // Hiện màu phần tranh vừa được đặt.
        portraitPart.color = Color.white;

        // Bật ánh sáng của trụ.
        if (beamEffect != null)
            beamEffect.SetActive(true);

        // Cập nhật giao diện hành trang.
        UIInventory inventoryUI = FindObjectOfType<UIInventory>();

        if (inventoryUI != null &&
            inventoryUI.gameObject.activeInHierarchy)
        {
            inventoryUI.Refresh();
        }

        // Đóng thông báo cũ trước khi mở Dialog.
        CancelInvoke(nameof(HideSuccessMessage));
        HideSuccessMessage();

        // Bật đối tượng Dialog tương ứng và phát nội dung ngay.
        // Chỉ hiện thông báo tại trụ.
        ShowMessage(
            "Đặt thành công",
            $"{displayItemName} đã được đặt thành công!"
        );

        // Mở vùng kích hoạt ở phần tranh.
        // Chưa phát Dialog tại đây.
        if (dialogueZone != null)
            dialogueZone.SetActive(true);

        // Báo trụ đã hoàn thành.
        if (centerController != null)
            centerController.RegisterPlacedPillar(this);

        Debug.Log(
            $"Đã đặt đúng mảnh {requiredItemName} vào {gameObject.name}.");
    }

    private void OpenPlacedDialogue()
    {
        // Trụ không cần Dialog có thể để trống Dialogue Zone.
        if (dialogueZone == null)
        {
            ShowMessage(
                "Đặt thành công",
                $"{displayItemName} đã được đặt thành công!"
            );
            return;
        }

        DialogueTrigger trigger =
            dialogueZone.GetComponent<DialogueTrigger>();

        if (trigger == null)
        {
            Debug.LogWarning(
                $"{dialogueZone.name}: chưa có component DialogueTrigger.");
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy DialogueManager trong Scene.");
            return;
        }

        if (trigger.GetDialogue() == null)
        {
            Debug.LogWarning(
                $"{dialogueZone.name}: chưa gán Dialogue Data.");
            return;
        }

        dialogueZone.SetActive(true);
        trigger.StartDialogue();
    }

    private void ShowMessage(string title, string message)
    {
        if (successPanel == null || successText == null)
        {
            Debug.LogWarning("Chưa gán bảng thông báo hoặc nội dung.");
            return;
        }

        if (successTitle != null)
            successTitle.text = title;

        successText.text = message;
        successPanel.SetActive(true);
        if (memoryBackground != null)
            memoryBackground.SetActive(false);

        CancelInvoke(nameof(HideSuccessMessage));
        Invoke(nameof(HideSuccessMessage), 2f);
    }

    private void HideSuccessMessage()
    {
        if (successPanel != null)
            successPanel.SetActive(false);
    }
}