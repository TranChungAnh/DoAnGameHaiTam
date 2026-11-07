using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Dedicated Map 1 chest UI, visually like inventory but NOT using UIInventory,
/// which is hardwired to the player's own StorageContainer.
/// Attach to an active Canvas in Map01, keep 'panel' initially inactive.
/// </summary>
public class Map1WarehouseChestUI : MonoBehaviour
{
    [Header("Panel va hai phan")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject lockedSection;
    [SerializeField] private GameObject contentsSection;
    [SerializeField] private GameObject emptyLabel;

    [Header("Khoa")]
    [SerializeField] private Image keyImage;
    [SerializeField] private TMP_Text keyCountText;
    [SerializeField] private Button unlockButton;

    [Header("Vat pham trong ruong")]
    [SerializeField] private Image artifactIcon;
    [SerializeField] private TMP_Text artifactNameText;
    [SerializeField] private Button takeArtifactButton;

    [Header("Dieu khien")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private Map1WarehouseChest currentChest;
    private PlayerController playerController;
    private bool previousCanMove;
    private bool movementFrozen;

    public bool IsOpen => panel != null && panel.activeSelf;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (unlockButton != null) unlockButton.onClick.AddListener(UnlockClicked);
        if (takeArtifactButton != null) takeArtifactButton.onClick.AddListener(TakeClicked);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    private void Update()
    {
        if (!IsOpen) return;
        if (currentChest == null || !currentChest.gameObject.activeInHierarchy)
        {
            Close();
            return;
        }
        Refresh();
    }

    public void Show(Map1WarehouseChest chest)
    {
        if (chest == null || panel == null) return;
        currentChest = chest;
        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null && !movementFrozen)
        {
            previousCanMove = playerController.canMove;
            playerController.canMove = false;
            movementFrozen = true;
        }
        panel.SetActive(true);
        Refresh();
    }

    public void Refresh()
    {
        if (currentChest == null) return;

        bool opened = currentChest.IsOpened;
        bool empty = currentChest.IsEmpty;
        bool hasKey = currentChest.HasKey();

        if (lockedSection != null) lockedSection.SetActive(!opened);
        if (contentsSection != null) contentsSection.SetActive(opened && !empty);
        if (emptyLabel != null) emptyLabel.SetActive(opened && empty);

        if (keyImage != null)
        {
            keyImage.sprite = currentChest.KeyIcon;
            keyImage.enabled = currentChest.KeyIcon != null;
        }
        if (keyCountText != null) keyCountText.text = hasKey ? "1/1" : "0/1";
        if (unlockButton != null) unlockButton.interactable = !opened && hasKey;

        if (artifactIcon != null)
        {
            ItemBase artifact = currentChest.Artifact;
            artifactIcon.sprite = artifact != null ? artifact.icon : null;
            artifactIcon.enabled = artifact != null && artifact.icon != null;
        }
        if (artifactNameText != null)
        {
            ItemBase artifact = currentChest.Artifact;
            artifactNameText.text = artifact != null ? artifact.itemName : "";
        }
        if (takeArtifactButton != null)
            takeArtifactButton.interactable = opened && !empty && currentChest.Artifact != null;

        if (messageText != null)
        {
            messageText.text = !opened
                ? (hasKey ? ""
                          : "")
                : (empty ? "" : "");
        }
    }

    private void UnlockClicked()
    {
        if (currentChest != null && currentChest.TryUnlock()) Refresh();
    }


    private void TakeClicked()
    {
        if (currentChest == null)
            return;

        // Ch? ti?p t?c khi ?ã nh?n c? v?t thành công
        if (!currentChest.TryTakeArtifact())
            return;

        // ?óng giao di?n r??ng và khôi ph?c tr?ng thái ?i?u khi?n
        Close();

        // Kích ho?t ký ?c C1-S08
        Map01Artifact01MemoryTrigger memoryTrigger =
            FindObjectOfType<Map01Artifact01MemoryTrigger>();

        if (memoryTrigger != null)
        {
            memoryTrigger.PlayAfterPickup();
        }
        else
        {
            Debug.LogWarning(
                "[Map01] Khong tim thay Map01Artifact01MemoryTrigger!"
            );
        }
    }

    public void CloseIfCurrent(Map1WarehouseChest chest)
    {
        if (currentChest == chest) Close();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (movementFrozen && playerController != null)
            playerController.canMove = previousCanMove;
        movementFrozen = false;
        playerController = null;
        currentChest = null;
    }

    private void OnDestroy()
    {
        if (movementFrozen && playerController != null)
            playerController.canMove = previousCanMove;
        if (unlockButton != null) unlockButton.onClick.RemoveListener(UnlockClicked);
        if (takeArtifactButton != null) takeArtifactButton.onClick.RemoveListener(TakeClicked);
        if (closeButton != null) closeButton.onClick.RemoveListener(Close);
    }
}
