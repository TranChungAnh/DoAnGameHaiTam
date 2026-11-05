
using UnityEngine;

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

    private void OnMouseDown()
    {
        if (paperCluePanel != null)
        {
            paperCluePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "Chưa gán PaperCluePanel cho ba lô!"
            );
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

