using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải có dòng này

public class ResearchStationTerminal : MonoBehaviour
{
    [SerializeField] private Puzzle2TerminalUI terminalUI;

    private void Update()
    {
        // Kiểm tra xem người dùng có vừa nhấn chuột phải hay không
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            // Bắn tia Raycast từ vị trí chuột để xem có bấm trúng trạm nghiên cứu này không
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // Kiểm tra va chạm 2D
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                OpenTerminal();
            }
        }
    }

    private void OpenTerminal()
    {
        Debug.Log("CLICK_RESEARCH_STATION_03");

        if (terminalUI == null)
        {
            Debug.LogError("Terminal UI chưa được gán!");
            return;
        }

        Debug.Log("Đang gọi mở bảng nhập mã.");
        terminalUI.OpenPanel();
    }
}