using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;

    public int slotNumber;
    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.Find("Text").GetComponent<TextMeshProUGUI>();

    }
    private void Update()
    {
        if (SaveManager.instance.isSlotEmpty(slotNumber))
        {
            buttonText.text = "";

        }
        else
        {
            buttonText.text = PlayerPrefs.GetString("Slot" + slotNumber + "Description");
        }
    }
    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            if (SaveManager.instance.isSlotEmpty(slotNumber) == false)
            {
                SaveManager.instance.StartLoadedGame(slotNumber);
                SaveManager.instance.DeselectButton();
            }
        });
    }
}
