using UnityEngine;

[System.Serializable]
public class SoilTileData
{
    public Vector2Int position;
    public bool isTilled = false;
    public bool isWatered = false;

    // Lưu tên cây trồng để biết ô đất này đang có cây gì
    public string plantedCropName = null;

    public float fertility = 1f;
    [Range(0f, 1f)]
    public float moisture=0f;
    [HideInInspector] public bool hasRainTimer = false;
    [HideInInspector] public float rainTimer = 0f;

}
