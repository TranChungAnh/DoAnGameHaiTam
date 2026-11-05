using UnityEngine;

/// <summary>
/// Camera follow tạm thời cho Map 2.
/// Dùng với camera Orthographic và player test.
/// </summary>
public class Map2TestCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;

    // Giới hạn camera theo map. Để false nếu chưa cần khóa biên.
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-60f, -36f);
    [SerializeField] private Vector2 maxBounds = new Vector2(59f, 35f);

    private Vector3 velocity;

    public Transform Target
    {
        get => target;
        set => target = value;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + new Vector3(0f, 0f, -10f);

        Camera cam = GetComponent<Camera>();
        if (cam != null && cam.orthographic && useBounds)
        {
            float halfHeight = cam.orthographicSize;
            float halfWidth = halfHeight * cam.aspect;

            float minX = minBounds.x + halfWidth;
            float maxX = maxBounds.x - halfWidth;
            float minY = minBounds.y + halfHeight;
            float maxY = maxBounds.y - halfHeight;

            if (minX <= maxX)
                desired.x = Mathf.Clamp(desired.x, minX, maxX);

            if (minY <= maxY)
                desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        desired.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref velocity,
            smoothTime
        );
    }
}
