using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Puzzle2TerminalUI : MonoBehaviour
{
    [Header("Puzzle")]
    [SerializeField] private Map2Puzzle2Manager puzzleManager;

    [Header("Panel")]
    [SerializeField] private GameObject panelRoot;

    [Header("Six digit slots")]
    [SerializeField] private Text[] digitSlots = new Text[6];

    [Header("Thông báo")]
    [SerializeField] private Text statusText;
    [SerializeField] private Color errorColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color successColor = new Color(0.4f, 1f, 0.5f);
    [SerializeField] private float wrongShowSeconds = 0.8f;
    [SerializeField] private float successDelay = 0.8f;

    [Header("Scene sau khi mở trạm")]
    [SerializeField] private string stationSceneName = "Map2B_ResearchStation03";

    private const int CodeLength = 6;
    private string enteredCode = "";
    private bool isBusy;

    private void Awake()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        RefreshSlots();
    }

    public void OpenPanel()
    {
        StopAllCoroutines();
        isBusy = false;
        enteredCode = "";
        ClearStatus();
        RefreshSlots();
        if (panelRoot != null) panelRoot.SetActive(true);
    }

    public void ClosePanel()
    {
        StopAllCoroutines();
        isBusy = false;
        if (panelRoot != null) panelRoot.SetActive(false);
    }

    public void PressDigit(int digit)
    {
        if (isBusy || digit < 0 || digit > 9) return;
        if (panelRoot == null || !panelRoot.activeInHierarchy) return;
        if (enteredCode.Length >= CodeLength) return;

        enteredCode += digit.ToString();
        ClearStatus();
        RefreshSlots();
    }

    public void Backspace()
    {
        if (isBusy || enteredCode.Length == 0) return;
        enteredCode = enteredCode.Substring(0, enteredCode.Length - 1);
        ClearStatus();
        RefreshSlots();
    }

    public void ClearCode()
    {
        if (isBusy) return;
        enteredCode = "";
        ClearStatus();
        RefreshSlots();
    }

    public void SubmitCode()
    {
        if (isBusy) return;

        if (enteredCode.Length != CodeLength)
        {
            ShowStatus("Vui lòng nhập đủ 6 chữ số.", errorColor);
            return;
        }

        if (puzzleManager == null)
        {
            Debug.LogError("Puzzle2TerminalUI: Chưa gán Puzzle Manager.");
            return;
        }

        if (puzzleManager.TrySubmitCode(enteredCode))
            StartCoroutine(SuccessRoutine());
        else
            StartCoroutine(WrongRoutine());
    }

    private IEnumerator WrongRoutine()
    {
        isBusy = true;
        ShowStatus("Mã mở khóa không chính xác.", errorColor);
        yield return new WaitForSecondsRealtime(wrongShowSeconds);
        enteredCode = "";
        RefreshSlots();          // dòng báo lỗi vẫn giữ đến khi người chơi bấm phím tiếp
        isBusy = false;
    }

    private IEnumerator SuccessRoutine()
    {
        isBusy = true;
        ShowStatus("Mã chính xác. Đang mở cửa...", successColor);
        yield return new WaitForSecondsRealtime(successDelay);
        ClosePanel();
        SceneManager.LoadScene(stationSceneName);
    }

    private void ShowStatus(string message, Color color)
    {
        if (statusText == null) return;
        statusText.color = color;
        statusText.text = message;
    }

    private void ClearStatus()
    {
        if (statusText != null) statusText.text = "";
    }

    private void RefreshSlots()
    {
        if (digitSlots == null) return;
        for (int i = 0; i < digitSlots.Length; i++)
        {
            if (digitSlots[i] == null) continue;
            digitSlots[i].text = i < enteredCode.Length ? enteredCode[i].ToString() : "";
        }
    }

    private void Update()
    {
        if (panelRoot != null && panelRoot.activeInHierarchy &&
            Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ClosePanel();
        }
    }
}