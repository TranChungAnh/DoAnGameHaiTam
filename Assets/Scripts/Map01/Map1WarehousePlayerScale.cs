
using UnityEngine;

public class Map1WarehousePlayerScale : MonoBehaviour
{
    [Header("Nhan vat")]
    [SerializeField] private Transform player;

    [Header("Ty le trong nha kho")]
    [SerializeField, Range(0.3f, 1f)]
    private float warehouseScale = 0.6f;

    private Vector3 originalScale;
    private bool? previousInside;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("[Map1] Chua gan Player!");
            enabled = false;
            return;
        }

        // Luu kich thuoc ban dau
        originalScale = player.localScale;
    }

    private void LateUpdate()
    {
        Map1WarehouseTravel travel =
            Map1WarehouseTravel.Instance;

        if (travel == null || travel.IsTransitioning)
            return;

        bool inside = travel.IsInside;

        // Chi thay doi khi vao hoac ra kho
        if (previousInside.HasValue &&
            previousInside.Value == inside)
            return;

        if (inside)
        {
            player.localScale = new Vector3(
                originalScale.x * warehouseScale,
                originalScale.y * warehouseScale,
                originalScale.z
            );
        }
        else
        {
            // Khoi phuc kich thuoc goc
            player.localScale = originalScale;
        }

        previousInside = inside;
    }
}
