using System.Collections.Generic;
using UnityEngine;

public class TempleCenterController : MonoBehaviour
{
    [Header("Số trụ cần hoàn thành")]
    [SerializeField] private int requiredPillars = 4;

    [Header("Cột sáng trung tâm bay lên trời")]
    [SerializeField] private GameObject centerBeam;
    [Header("Cột sáng trung tâm bay lên trời")]
    [SerializeField] private GameObject centerBeam1;
    [Header("Cổ vật 6 xuất hiện")]
    [SerializeField] private GameObject artifact6;

    [Header("Hiện Dialog")]
    [SerializeField] private GameObject Dialogzone;

    private readonly HashSet<BossFragmentSlot> completedPillars =
        new HashSet<BossFragmentSlot>();

    private bool isActivated;

    private void Awake()
    {
        if (centerBeam != null)
            centerBeam.SetActive(false);
        if (centerBeam1 != null)
            centerBeam1.SetActive(false);
        if (artifact6 != null)
            artifact6.SetActive(false);
        if (Dialogzone != null)
            Dialogzone.SetActive(false);
    }

    public void RegisterPlacedPillar(BossFragmentSlot pillar)
    {
        if (isActivated || pillar == null)
            return;

        // Một trụ chỉ được tính một lần.
        if (!completedPillars.Add(pillar))
            return;

        if (completedPillars.Count >= requiredPillars)
        {
            isActivated = true;

            if (centerBeam != null)
                centerBeam.SetActive(true);
            if (centerBeam1 != null)
                centerBeam1.SetActive(true);
            if (artifact6 != null)
                artifact6.SetActive(true);
            if (Dialogzone != null)
                Dialogzone.SetActive(true);
        }
    }
}