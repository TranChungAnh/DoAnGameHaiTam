using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có dòng này

public class BackpackPaperInteraction : MonoBehaviour
{
    [Header("Giao diện mẩu giấy")]
    [SerializeField] private GameObject paperCluePanel;

    private void Start()
    {
        if (paperCluePanel != null)
        {
            paperCluePanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Thay đổi từ leftButton sang rightButton để nhận diện chuột phải
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (Camera.main == null)
            {
                Debug.LogWarning("[Click Debug] Không tìm thấy Camera.main!");
                return;
            }

            // 1. Vị trí chuột trên màn hình (pixel)
            Vector2 mousePos = Mouse.current.position.ReadValue();

            // 2. Quy đổi sang tọa độ thế giới 2D (truyền khoảng cách từ camera tới mặt phẳng z=0)
            Vector3 screenWithDepth = new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z);
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenWithDepth);

            // 3. Kiểm tra va chạm 2D và in ra chỗ đang click
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"[Click Debug] Screen: {mousePos} | World: {worldPos} -> ĐANG TRÚNG: '{hit.collider.gameObject.name}' (Ba lô của script này là: '{gameObject.name}')");

                if (hit.collider.gameObject == gameObject)
                {
                    OpenPaper();
                }
            }
            else
            {
                Debug.Log($"[Click Debug] Screen: {mousePos} | World: {worldPos} -> ĐANG CLICK VÀO KHOẢNG TRỐNG (không trúng Collider 2D nào)");
            }
        }
    }

    public void OpenPaper()
    {
        if (paperCluePanel != null)
        {
            paperCluePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Chưa gán PaperCluePanel cho ba lô!");
        }
    }

    public void ClosePaper()
    {
        if (paperCluePanel != null)
        {
            paperCluePanel.SetActive(false);
        }
    }
}