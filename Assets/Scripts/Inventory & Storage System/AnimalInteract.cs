using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animal))]
public class AnimalInteract : MonoBehaviour
{
    private Animal animal;
    private Camera mainCam;
    public  bool playerInRange = false;

    void Awake()
    {
        animal = GetComponent<Animal>();
        mainCam = Camera.main;
    }

    void Update()
    {
        if (!playerInRange) return; 

        // Mobile (touch)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            TryInteract(touchPos);
        }

        // PC (mouse)
        if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector2 clickPos = Mouse.current.position.ReadValue();
            TryInteract(clickPos);
        }
    }

    private void TryInteract(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Debug.Log($"Interacted with {animal.data.animalName} at {Time.time} seconds.");
            FeedingManager.Instance.CareAnimal(animal);
            ProduceManager.Instance.CollectProduce(animal);
        }
    }

    // Check player va chạm collider vật lý (IsTrigger = false)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log($"Player entered range of {animal.data.animalName} at {Time.time} seconds.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
