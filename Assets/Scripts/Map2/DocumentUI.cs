
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DocumentUI : MonoBehaviour
{
    public static DocumentUI Instance { get; private set; }

    [Header("Document UI")]
    [SerializeField] private GameObject documentPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Button closeButton;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (documentPanel != null)
            documentPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDocument);
    }

    private void Update()
    {
        if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseDocument();
    }

    public void OpenDocument(string title, string content)
    {
        if (documentPanel == null)
        {
            Debug.LogError("DocumentUI: Chưa gán DocumentPanel.");
            return;
        }

        titleText.text = title;
        contentText.text = content;

        documentPanel.SetActive(true);
        IsOpen = true;

        Time.timeScale = 0f;
    }

    public void CloseDocument()
    {
        if (documentPanel != null)
            documentPanel.SetActive(false);

        IsOpen = false;
        Time.timeScale = 1f;
    }
}
