using System;
using UnityEngine;
using static UnityEditor.Progress;

public static class FarmingEvents
{
    public static Action<CropData> OnCropPlanted;
    public static Action<Item> OnCropHarvested;
}
