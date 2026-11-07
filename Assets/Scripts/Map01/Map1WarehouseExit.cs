using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach to the interior exit-door GameObject alongside its Collider2D.
/// Supports F (near door) and right-click interaction (if InputSystem hits it).
/// No Inspector reference to the exterior scene is necessary.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Map1WarehouseExit : MonoBehaviour, IInteractable
{
    [SerializeField, Min(0.1f)] private float interactionDistance = 1.5f;
    [SerializeField] private bool allowFKey = true;

    private Transform player;

    private void Update()
    {
        if (!allowFKey || Keyboard.current == null ||
            !Keyboard.current.fKey.wasPressedThisFrame) return;

        Interact();
    }

    public void Interact()
    {
        Map1WarehouseTravel travel = Map1WarehouseTravel.Instance;
        if (travel == null || travel.IsTransitioning || !travel.IsInside) return;
        if (!IsPlayerInRange()) return;
        travel.ExitWarehouse();
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            if (controller != null) player = controller.transform;
        }
        return player != null &&
               ((Vector2)(player.position - transform.position)).sqrMagnitude
               <= interactionDistance * interactionDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
