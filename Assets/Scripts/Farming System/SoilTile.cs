using UnityEngine;

[System.Serializable]
public class SoilTile 
{
    public SoilTileData data;

    public SoilTile(Vector2Int pos)
    {
        data = new SoilTileData { position = pos };
    }

    /// Cày đất
    public void Till()
    {
        if (!data.isTilled)
        {
        data.isTilled = true;
            SoilManager.Instance.SetTileVisual(data.position, true,data.isWatered);
        }
    }

    /// Trồng cây trên đất đã cày
    public bool  PlantCrop(CropData crop)
    {
        if (!data.isTilled || !string.IsNullOrEmpty(data.plantedCropName)) 
            return false;

        Vector3Int cell = (Vector3Int)data.position;

        var planted = CropManager.Instance.PlantCrop(
            SoilManager.Instance.groundTilemap.GetCellCenterWorld(cell),
            crop.resourceName 
        );

        if (planted != null)
        {
            data.plantedCropName = crop.resourceName;
            return true; // ✅ thành công
        }

        return false;
    }



    public void Water()
    {
        // chỉ cho tưới nếu đã cuốc đất
        if (!data.isTilled)
            return;

        if (data.moisture < 1f)
        {
            data.isWatered = true;
            data.moisture = Mathf.Clamp01(data.moisture + 0.5f); // tăng 50% mỗi lần tưới
            Debug.Log($"Đã tưới ô {data.position}, độ ẩm hiện tại: {data.moisture * 100f}%");
            SoilManager.Instance.SetTileVisual(data.position, data.isTilled, data.isWatered);
        }
    }

    public void UpdateMoisture(float deltaTime)
    {
        if (data.moisture > 0f)
        {
            data.moisture = Mathf.Max(0f, data.moisture - deltaTime * 0.005f);

            bool wasWatered = data.isWatered;
            data.isWatered = data.moisture > 0f;

            if (wasWatered && !data.isWatered)
            {
                SoilManager.Instance.SetTileVisual(data.position, data.isTilled, data.isWatered);
            }
        }

     
    }

    /// Qua ngày mới
    public void NewDay()
    {
        data.isWatered = false;
        data.isWatered = data.moisture > 0f;
        data.fertility = Mathf.Max(0f, data.fertility - 0.01f);

    }

    /// Kiểm tra ô đất có cây không
    public bool HasCrop()
    {
        return !string.IsNullOrEmpty(data.plantedCropName);
    }

    /// Xóa cây (khi thu hoạch)
    public void ClearCrop()
    {
        data.plantedCropName = null;
    }
}
