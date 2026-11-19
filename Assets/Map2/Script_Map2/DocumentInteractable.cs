using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có để dùng New Input System

public class DocumentInteractable : MonoBehaviour
{
    [TextArea(3, 15)]
    [SerializeField] private string documentTitle = "BIÊN BẢN THỬ NGHIỆM";

    [TextArea(5, 30)]
    [SerializeField]
    private string documentContent = "Nội dung hồ sơ cần hiển thị ở đây.";

    private void Update()
    {
        // 1. Kiểm tra xem giao diện Document đã mở chưa và có bấm chuột phải hay không
        if (DocumentUI.Instance != null && DocumentUI.Instance.IsOpen) return;
        if (Mouse.current == null || !Mouse.current.rightButton.wasPressedThisFrame) return;

        // 2. Bắn tia Raycast từ vị trí chuột để đảm bảo click đúng vào mẩu tài liệu này
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider == null) return;

        // Kiểm tra xem đối tượng bị click có chứa script này hoặc nằm trên cùng một vật thể không
        DocumentInteractable clickedDoc = hit.collider.GetComponent<DocumentInteractable>();
        if (clickedDoc == null || clickedDoc != this) return;

        // 3. Mở tài liệu nếu thỏa mãn tất cả điều kiện
        if (DocumentUI.Instance != null)
        {
            DocumentUI.Instance.OpenDocument(documentTitle, documentContent);
        }
        else
        {
            Debug.LogWarning("DocumentUI.Instance chưa tồn tại trong Scene!");
        }
    }
}