using UnityEngine;
using UnityEngine.Events;

public class Map2Puzzle2Manager : MonoBehaviour
{
    [Header("Puzzle 2")]
    [SerializeField] private int requiredDigits = 6;

    [Header("Requirements")]
    [SerializeField] private bool requireLargeEnemyDefeated = true;
    [SerializeField] private bool requireCodeMap = true;

    [Header("Correct Code")]
    [SerializeField] private string correctCode = "";

    [Header("Events")]
    public UnityEvent OnAllDigitsCollected;
    public UnityEvent OnLargeEnemyDefeated;
    public UnityEvent OnCodeMapObtained;
    public UnityEvent OnPuzzleReady;
    public UnityEvent OnPuzzleSolved;

    private bool[] digitsCollected;
    private bool largeEnemyDefeated;
    private bool codeMapObtained;
    private bool solved;

    public bool IsSolved => solved;

    public bool IsReady
    {
        get
        {
            return AllDigitsCollected()
                && (!requireLargeEnemyDefeated || largeEnemyDefeated)
                && (!requireCodeMap || codeMapObtained);
        }
    }

    private void Awake()
    {
        requiredDigits = Mathf.Clamp(requiredDigits, 1, 10);

        digitsCollected = new bool[requiredDigits];

        correctCode = NormalizeCode(correctCode);
    }

    public void CollectDigit(int index)
    {
        if (solved)
            return;

        if (index < 0 || index >= requiredDigits)
            return;

        if (digitsCollected[index])
            return;

        digitsCollected[index] = true;

        if (AllDigitsCollected())
        {
            OnAllDigitsCollected?.Invoke();
        }

        CheckReady();
    }

    public void MarkLargeEnemyDefeated()
    {
        if (solved)
            return;

        largeEnemyDefeated = true;

        OnLargeEnemyDefeated?.Invoke();

        CheckReady();
    }

    public void ObtainCodeMap()
    {
        if (solved)
            return;

        codeMapObtained = true;

        OnCodeMapObtained?.Invoke();

        CheckReady();
    }


    public bool TrySubmitCode(string enteredCode)
    {
        if (solved) return false;

        enteredCode = NormalizeCode(enteredCode);
        correctCode = NormalizeCode(correctCode);

       
        if (enteredCode.Length != 6) return false;

      
        if (string.IsNullOrEmpty(correctCode))
        {
            Debug.LogError("Puzzle 2: Chưa cấu hình Correct Code.");
            return false;
        }

      
        if (enteredCode != correctCode) return false;

        solved = true;
        OnPuzzleSolved?.Invoke();
        return true;
    }

    public bool IsDigitCollected(int index)
    {
        if (index < 0 || index >= digitsCollected.Length)
            return false;

        return digitsCollected[index];
    }

    private bool AllDigitsCollected()
    {
        for (int i = 0; i < digitsCollected.Length; i++)
        {
            if (!digitsCollected[i])
                return false;
        }

        return true;
    }

    private void CheckReady()
    {
        if (IsReady)
        {
            OnPuzzleReady?.Invoke();
        }
    }

    private string NormalizeCode(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        string result = "";

        foreach (char c in value)
        {
            if (char.IsDigit(c))
            {
                result += c;
            }
        }

        return result;
    }
}