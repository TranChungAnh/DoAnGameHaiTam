using System.Collections.Generic;
using UnityEngine;

public class FinalArtifactController : MonoBehaviour
{
    [Header("Số bệ cần đặt đủ")]
    [SerializeField] private int requiredSlots = 6;

    [Header("Cổ vật 7 ở trung tâm")]
    [SerializeField] private GameObject artifact7;

    [Header("Hiệu ứng trung tâm - có thể để trống")]
    [SerializeField] private GameObject centerEffect;

    [Header("Hiệu ứng trung tâm - có thể để trống1")]
    [SerializeField] private GameObject centerEffect1;

    private readonly HashSet<FinalArtifactSlot> completedSlots =
        new HashSet<FinalArtifactSlot>();

    private bool isActivated;

    private void Awake()
    {
        // Ban đầu chưa xuất hiện cổ vật 7.
        if (artifact7 != null)
            artifact7.SetActive(false);

        if (centerEffect != null)
            centerEffect.SetActive(false);

        if(centerEffect1 != null)
            centerEffect1.SetActive(false);
    }

    public void RegisterPlacedSlot(FinalArtifactSlot slot)
    {
        if (isActivated || slot == null)
            return;

        // Mỗi bệ chỉ được tính một lần.
        if (!completedSlots.Add(slot))
            return;

        Debug.Log(
            $"Đã đặt cổ vật: {completedSlots.Count}/{requiredSlots}");

        if (completedSlots.Count < requiredSlots)
            return;

        isActivated = true;

        // Đủ 6 bệ thì cổ vật 7 xuất hiện.
        if (artifact7 != null)
            artifact7.SetActive(true);

        if (centerEffect != null)
            centerEffect.SetActive(true);

        if (centerEffect1 != null)
            centerEffect1.SetActive(true);
        Debug.Log("Đã đặt đủ 6 cổ vật. Cổ vật 7 xuất hiện!");
    }
}