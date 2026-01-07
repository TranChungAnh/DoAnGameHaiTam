using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputAction input;

    private void Awake()
    {
        input = new PlayerInputAction();
    }

    private void OnEnable()
    {
        input.Gameplay.Enable();
        input.Gameplay.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        input.Gameplay.Interact.performed -= OnInteract;
        input.Gameplay.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("INTERACT");
    }
}
