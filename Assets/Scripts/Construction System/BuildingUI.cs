using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject buildingSlotPrefab;
    public BuildingData[] allBuildings;

    private void Start()
    {
        foreach (var building in allBuildings)
        {
            GameObject slot = Instantiate(buildingSlotPrefab, contentParent);
            var iconImg = slot.transform.Find("Icon").GetComponent<Image>();
            var nameText = slot.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            var recipeText = slot.transform.Find("Recipe").GetComponent<TextMeshProUGUI>();
            var buildBtn = slot.transform.Find("BuildButton").GetComponent<Button>();

            iconImg.sprite = building.icon;
            nameText.text = building.buildingName;

            StringBuilder sb = new StringBuilder();
            foreach (var entry in building.recipe)
                sb.AppendLine($"{entry.item.itemName} x{entry.amount}");
            recipeText.text = sb.ToString();

            BuildingData copy = building;
            buildBtn.onClick.AddListener(() => OnBuildClicked(copy));
        }
    }

    private void OnBuildClicked(BuildingData building)
    {
        BuildingManager.Instance.StartPlacing(building);
        foreach(Transform child in transform.root.GetComponentsInChildren<Transform>())
        {
            var canvasGroup=child.GetComponent<CanvasGroup>();
            var go = child.gameObject;

            if(go!=gameObject&& go.CompareTag("UIPanel"))
            {
                go.SetActive(false);
            }
        }
    }
}
