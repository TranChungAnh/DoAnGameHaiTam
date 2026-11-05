using UnityEngine;

public class Puzzle2LargeEnemyReward : MonoBehaviour
{
    [SerializeField] private Map2Puzzle2Manager puzzleManager;

    [Header("Map hướng dẫn")]
    [SerializeField] private GameObject codeMap;

    private bool claimed;

    private void Awake()
    {
        if (codeMap != null)
        {
            codeMap.SetActive(false);
        }
    }

    public void OnLargeEnemyDefeated()
    {
        if (claimed)
            return;

        if (puzzleManager == null)
            return;

        claimed = true;

        puzzleManager.MarkLargeEnemyDefeated();

        if (codeMap != null)
        {
            codeMap.SetActive(true);
        }

        puzzleManager.ObtainCodeMap();
    }

    public void CollectMap()
    {
        if (puzzleManager == null)
            return;

        puzzleManager.ObtainCodeMap();

        if (codeMap != null)
        {
            codeMap.SetActive(false);
        }
    }
}