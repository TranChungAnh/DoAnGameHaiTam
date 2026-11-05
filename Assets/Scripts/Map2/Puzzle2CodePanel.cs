using UnityEngine;
using UnityEngine.Events;

public class Puzzle2CodePanel : MonoBehaviour
{
    [SerializeField] private Map2Puzzle2Manager puzzleManager;

    [SerializeField] private int codeLength = 6;

    [Header("Events")]
    public UnityEvent OnCorrectCode;
    public UnityEvent OnWrongCode;
    public UnityEvent OnNotReady;

    private string enteredCode = "";

    public void PressDigit(int digit)
    {
        if (digit < 0 || digit > 9)
            return;

        if (enteredCode.Length >= codeLength)
            return;

        enteredCode += digit.ToString();

        Debug.Log("Puzzle 2 Code: " + enteredCode);
    }

    public void ClearCode()
    {
        enteredCode = "";
    }

    public void Backspace()
    {
        if (enteredCode.Length == 0)
            return;

        enteredCode =
            enteredCode.Substring(
                0,
                enteredCode.Length - 1
            );
    }

    public void Submit()
    {
        if (puzzleManager == null)
            return;

        if (!puzzleManager.IsReady)
        {
            OnNotReady?.Invoke();
            return;
        }

        bool correct =
            puzzleManager.TrySubmitCode(enteredCode);

        if (correct)
        {
            OnCorrectCode?.Invoke();
        }
        else
        {
            OnWrongCode?.Invoke();
        }
    }
}