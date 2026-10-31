using System.Collections;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    [Header("Cấu hình Animation")]
    public string animationStateName;
    private Animator animator;

    [Header("Cấu hình Di chuyển (Từ Điểm A đến B)")]
    public Transform startPoint; // Điểm xuất phát (có thể để trống, nó sẽ lấy vị trí hiện tại)
    public Transform endPoint;   // Điểm đích đến
    public float moveSpeed = 3f; // Tốc độ di chuyển
    public bool destroyWhenReached = true; // Tự hủy khi đến đích

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // 1. Đặt vị trí bắt đầu nếu có gán startPoint
        if (startPoint != null)
        {
            transform.position = startPoint.position;
        }

        // 2. Kích hoạt Animation đã chọn
        if (animator != null && !string.IsNullOrEmpty(animationStateName))
        {
            animator.Play(animationStateName);
        }
        else
        {
            Debug.LogWarning($"⚠ {gameObject.name} chưa gán tên Animation hoặc thiếu Animator!");
        }

        // 3. Bắt đầu di chuyển đến điểm đích
        if (endPoint != null)
        {
            StartCoroutine(MoveToTargetRoutine());
        }
        else
        {
            Debug.LogWarning($"⚠ {gameObject.name} chưa gán điểm đến (EndPoint)!");
        }
    }

    private IEnumerator MoveToTargetRoutine()
    {
        // Lặp lại cho đến khi di chuyển sát đến điểm đích
        while (endPoint != null && Vector3.Distance(transform.position, endPoint.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPoint.position, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Khi đã đến đích
        if (destroyWhenReached)
        {
            Destroy(gameObject);
        }
    }
}