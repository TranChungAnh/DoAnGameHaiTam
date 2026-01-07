using UnityEngine;

public class ProduceManager : MonoBehaviour
{
    public static ProduceManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void CollectProduce(Animal animal)
    {
        // Duyệt qua tất cả sản phẩm mà animal có thể sản xuất
        foreach (var produce in animal.data.produces)
        {
            // Chuyển tên produceName (Milk, Wool, Egg...) về Enum AnimalStateType nếu có
            if (System.Enum.TryParse(produce.produceName, out AnimalStateType stateType))
            {
                // Nếu trạng thái này đang active thì tiến hành thu hoạch
                if (animal.IsStateActive(stateType))
                {
                    animal.HarvestProduce(produce);
                }
            }
        }
    }
}
