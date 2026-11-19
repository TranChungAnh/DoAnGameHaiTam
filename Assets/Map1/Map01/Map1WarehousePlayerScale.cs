using UnityEngine;
using Cinemachine; // Nhớ thêm dòng này để điều khiển Cinemachine

public class Map1WarehousePlayerScale : MonoBehaviour
{
    [Header("Nhan vat")]
    [SerializeField] private Transform player;

    [Header("Cinemachine Virtual Camera (Tuy chon)")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // Kéo Virtual Camera vào đây

    [Header("Ty le trong nha kho")]
    [SerializeField, Range(0.3f, 1f)]
    private float warehouseScale = 0.6f;

    private Vector3 originalScale;
    private float originalOrthoSize;
    private bool? previousInside;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("[Map1] Chua gan Player!");
            enabled = false;
            return;
        }

        // Luu kich thuoc ban dau cua Player
        originalScale = player.localScale;

        // Luu Orthographic Size ban dau cua Virtual Camera
        if (virtualCamera != null)
        {
            originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        }
        else
        {
            // Tự động tìm nếu quên kéo vào Inspector
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
            }
        }
    }

    private void LateUpdate()
    {
        Map1WarehouseTravel travel = Map1WarehouseTravel.Instance;

        if (travel == null || travel.IsTransitioning)
            return;

        bool inside = travel.IsInside;

        // Chi thay doi khi vao hoac ra kho
        if (previousInside.HasValue &&
            previousInside.Value == inside)
            return;

        if (inside)
        {
            // Thu nho Player
            player.localScale = new Vector3(
                originalScale.x * warehouseScale,
                originalScale.y * warehouseScale,
                originalScale.z
            );

            // Thu nho Virtual Camera (Orthographic Size)
            if (virtualCamera != null)
            {
                virtualCamera.m_Lens.OrthographicSize = originalOrthoSize * warehouseScale;
            }
        }
        else
        {
            // Khoi phuc kich thuoc goc cua Player
            player.localScale = originalScale;

            // Khoi phuc Virtual Camera Orthographic Size
            if (virtualCamera != null)
            {
                virtualCamera.m_Lens.OrthographicSize = originalOrthoSize;
            }
        }

        previousInside = inside;
    }
}