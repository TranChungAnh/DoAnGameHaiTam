using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public int slotNumber;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        // 🔹 Cập nhật hiển thị ngay khi bắt đầu mở giao diện Save Menu
        UpdateSlotUI();

        button.onClick.AddListener(() =>
        {
            // 🔹 Cho phép bấm để lưu luôn (hoặc bạn có thể thêm bảng cảnh báo ghi đè ở đây nếu muốn)
            SaveGameConfirmed();
        });
    }

    public void UpdateSlotUI()
    {
        if (SaveManager.instance.isSlotEmpty(slotNumber))
        {
            buttonText.text = "Empty Slot " + slotNumber;
        }
        else
        {
            // Lấy mô tả đã lưu, nếu không có thì hiện tên mặc định
            string defaultDesc = "Saved Game " + slotNumber;
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description", defaultDesc);
        }
    }

    private void SaveGameConfirmed()
    {
        // 1. Thực hiện lưu game qua SaveManager
        SaveManager.instance.SaveGame(slotNumber);

        // 2. Tạo chuỗi thời gian hiện tại
        DateTime dt = DateTime.Now;
        string time = dt.ToString("yyyy-MM-dd HH:mm");

        // 3. Lưu mô tả vào PlayerPrefs để hiển thị
        string description = "Slot " + slotNumber + " | " + time;
        PlayerPrefs.SetString("Slot" + slotNumber + "Description", description);
        PlayerPrefs.Save(); // Đảm bảo ghi dữ liệu xuống ổ cứng ngay lập tức

        // 4. Cập nhật lại giao diện ngay lập tức
        buttonText.text = description;

        // 5. Bỏ chọn button tránh bị dính hiệu ứng sáng (Select)
        SaveManager.instance.DeselectButton();

        Debug.Log($"✅ Đã lưu thành công vào Slot {slotNumber}!");
    }
}