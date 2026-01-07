using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    public static HotbarManager Instance { get; private set; }

    [Header("UI Hotbar Slots")]
    public UIHotbarSlot[] hotbarSlots;
    public int selectedIndex = -1;

    private ItemSpawner spawner;
    public ItemBase SselectedItem { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        var player = GameObject.FindGameObjectWithTag("Player");
        spawner = player.GetComponent<ItemSpawner>();

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i] != null)
                hotbarSlots[i].slotIndex = i;
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        // --- PC: Dùng số 1-9 để chọn hotbar ---
        if (Keyboard.current != null)
        {
            for (int i = 0; i < hotbarSlots.Length; i++)
            {
                if (Keyboard.current[(Key)((int)Key.Digit1 + i)].wasPressedThisFrame)
                {
                    SelectSlot(i);
                }
            }
        }

        // --- Mobile: chạm trực tiếp vào slot UI (đã có OnPointerClick) ---
        // Không cần xử lý riêng, đã gọi OnHotbarItemClicked()
    }

    public void OnHotbarItemClicked(ItemBase item, int index)
    {
        SelectSlot(index);
    }

    public void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSlots.Length) return;

        selectedIndex = index;
        var slot = hotbarSlots[index].boundSlot;

        if (slot != null && !slot.IsEmpty )
        {
            SselectedItem = slot.item;
            if (spawner != null)
            {
            spawner.SpawnItem(slot.item);
            }
        }
        else
        {
            SselectedItem = null;
            if (spawner != null)
            {
                spawner.ClearItem();
            }
        }
    }
}
