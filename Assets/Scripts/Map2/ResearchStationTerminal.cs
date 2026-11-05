
using UnityEngine;

public class ResearchStationTerminal : MonoBehaviour
{
    [SerializeField] private Puzzle2TerminalUI terminalUI;

    private void OnMouseDown()
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