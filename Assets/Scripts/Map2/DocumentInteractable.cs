
using UnityEngine;

public class DocumentInteractable : MonoBehaviour
{
    [TextArea(3, 15)]
    [SerializeField] private string documentTitle = "BIÊN BẢN THỬ NGHIỆM";

    [TextArea(5, 30)]
    [SerializeField]
    private string documentContent =
        "Nội dung hồ sơ cần hiển thị ở đây.";

    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerNearby;

    private void Update()
    {
        if (playerNearby &&
            !DocumentUI.Instance.IsOpen &&
            Input.GetKeyDown(interactKey))
        {
            DocumentUI.Instance.OpenDocument(
                documentTitle,
                documentContent);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}
