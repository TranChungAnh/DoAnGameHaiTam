using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InteractionSystem interactionSystem;

    public void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("CALLBACK PHASE: " + context.phase);

        if (!context.performed) return;

        Debug.Log("INTERACT PERFORMED");
        interactionSystem.TryClickInteract();
    }


}
