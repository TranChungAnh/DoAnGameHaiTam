using UnityEngine;

public class FeedingManager : MonoBehaviour
{
    public static FeedingManager Instance { get; private set; }

    [Header("Kho/túi người chơi")]
    public StorageContainer playerInventory;

    [Header("Tên thức ăn mặc định (cho Hungry)")]
    [SerializeField] private string defaultFoodName = "Lúa";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Chăm sóc con vật: ưu tiên chữa bệnh/khát, nếu không thì cho ăn
    /// </summary>
    public void CareAnimal(Animal animal)
    {
        bool cured = false;

        // 1. chữa các trạng thái khác (không phải Hungry)
        foreach (var s in animal.data.states)
        {
            if (s.stateType != AnimalStateType.Hungry && animal.IsStateActive(s.stateType))
            {
                if (playerInventory.HasItem(s.cureItemName))
                {
                    playerInventory.RemoveItem(s.cureItemName, 1);
                    animal.CureState(s.stateType);
                    Debug.Log($"{animal.data.animalName} đã được chữa {s.stateType} bằng {s.cureItemName}!");
                    cured = true;
                    break;
                }
            }
        }

        // 2. nếu chưa chữa gì, thử cho ăn
        if (!cured && animal.IsStateActive(AnimalStateType.Hungry))
        {
            if (playerInventory.HasItem(defaultFoodName))
            {
                playerInventory.RemoveItem(defaultFoodName, 1);
                animal.CureState(AnimalStateType.Hungry);
                Debug.Log($"{animal.data.animalName} đã được cho ăn bằng {defaultFoodName}!");
            }
            else
            {
                Debug.Log("⚠ Không có thức ăn trong kho!");
            }
        }

        // 3. nếu không cần gì hết
        if (!cured && !animal.IsStateActive(AnimalStateType.Hungry))
        {
            Debug.Log($"{animal.data.animalName} hiện không cần chăm sóc gì!");
        }
    }
}
