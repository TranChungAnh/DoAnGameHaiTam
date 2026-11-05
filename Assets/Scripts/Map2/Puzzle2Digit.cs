using UnityEngine;

public class Puzzle2Digit : MonoBehaviour
{
    [Header("Puzzle Manager")]
    [SerializeField] private Map2Puzzle2Manager puzzleManager;

    [Header("Digit")]
    [SerializeField] private int digitIndex;
    [SerializeField] private int digitValue;

    [Header("After Collect")]
    [SerializeField] private bool hideAfterCollected = true;

    private bool collected;

    public int DigitIndex => digitIndex;
    public int DigitValue => digitValue;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Collect();
    }

    public void Collect()
    {
        if (collected)
            return;

        if (puzzleManager == null)
        {
            Debug.Log(
    $"Puzzle 2: Đã thu thập Digit {digitIndex + 1} = {digitValue}"
);


            return;
        }

        collected = true;

        puzzleManager.CollectDigit(digitIndex);

        if (hideAfterCollected)
        {
            gameObject.SetActive(false);
        }
    }
}