[System.Serializable]
public class StorageSlot
{
    public ItemBase item;
    public int quantity;

    public bool IsEmpty => item == null || quantity <= 0;

    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}
