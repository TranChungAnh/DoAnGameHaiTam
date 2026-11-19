
using UnityEngine;

public class InteractionPrompt : MonoBehaviour
{
    [Header("GameObject hiển thị")]
    [SerializeField] private GameObject promptObject;

    [Header("Cấu hình vùng nhận diện")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool useLayerFilter = false;

    [SerializeField] private BoxCollider2D triggerCollider;

    private void Awake()
    {
        // Lấy hoặc tự động thêm BoxCollider2D
        triggerCollider = GetComponent<BoxCollider2D>();

        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        triggerCollider.isTrigger = true;

        // Ẩn icon ban đầu
        if (promptObject != null)
        {
            promptObject.SetActive(false);
        }
    }

    private bool IsPlayer(Collider2D collision)
    {
        if (useLayerFilter)
        {
            return ((1 << collision.gameObject.layer) & playerLayer) != 0;
        }

        return collision.CompareTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            if (promptObject != null)
                promptObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            if (promptObject != null)
                promptObject.SetActive(false);
        }
    }

    // Gizmos hiển thị vùng Trigger
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();

        if (col != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(col.offset, col.size);
        }
    }
}
