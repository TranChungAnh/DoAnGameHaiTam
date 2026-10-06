
using UnityEngine;
using UnityEngine.InputSystem;
public class InteractionSystem : MonoBehaviour
{
    [SerializeField] private LayerMask interactLayer;
    public void TryClickInteract()
    {

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        RaycastHit2D hit = Physics2D.Raycast(
            worldPos,
            Vector2.zero
        );

        if (!hit) return;

        hit.collider.GetComponent<IInteractable>()?.Interact();
    }

}
