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
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.instance.isSlotEmpty(slotNumber))
            {
                SaveGameConfirmed();
            }
           
        });
    }
    public void Update()
    {
        if (SaveManager.instance.isSlotEmpty(slotNumber))
        {
            buttonText.text = "Empty";
        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }
    //public void DisplayOverrideWarning()
    //{
    //    SaveGameConfirmed();

    //}
    private void SaveGameConfirmed()
    {
        SaveManager.instance.SaveGame(slotNumber);
        DateTime dt = DateTime.Now;
        string time = dt.ToString("yyyy-MM-dd HH:mm");

        string description = "Saved Game " + slotNumber + " | " + time;
        buttonText.text = description;
        PlayerPrefs.SetString("Slot" + slotNumber + "Description", description);
        SaveManager.instance.DeselectButton();

    }
}