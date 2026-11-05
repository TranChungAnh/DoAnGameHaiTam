using UnityEngine;

/// <summary>
/// Six-digit access terminal for Research Station 03.
/// The code is the same six-digit sequence documented for Map 2: 123456.
/// This is a security-access puzzle, not a power/switch puzzle.
/// </summary>
public class ResearchStationAccessCode : MonoBehaviour
{
    public string requiredCode = "123456";
    public string persistenceKey = "HT_RS03_AccessGranted";
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";
    public bool persistAcrossLoads = true;
    public GameObject[] unlockOnSuccess;
    public GameObject[] enableOnSuccess;

    private bool playerInside;
    private bool solved;
    private bool keypadOpen;
    private string entered = "";
    private string status = "";
    private float statusUntil;
    private GUIStyle panelStyle;
    private GUIStyle titleStyle;
    private GUIStyle displayStyle;
    private GUIStyle buttonStyle;

    public bool IsSolved => solved;

    private void Awake()
    {
        if (persistAcrossLoads && PlayerPrefs.GetInt(persistenceKey, 0) == 1)
            ApplySolvedState();
    }

    private void Update()
    {
        if (!playerInside || solved) return;

        if (Input.GetKeyDown(interactKey))
            keypadOpen = !keypadOpen;

        if (keypadOpen)
        {
            string input = Input.inputString;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c >= '0' && c <= '9' && entered.Length < requiredCode.Length)
                    entered += c;
                else if (c == '\b' && entered.Length > 0)
                    entered = entered.Substring(0, entered.Length - 1);
                else if (c == '\n' || c == '\r')
                    SubmitCode();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerInside = false;
        keypadOpen = false;
        entered = "";
    }

    public void SubmitCode()
    {
        if (solved) return;

        if (entered == requiredCode)
        {
            solved = true;
            keypadOpen = false;
            entered = "";
            status = "ACCESS GRANTED";
            statusUntil = Time.unscaledTime + 2.0f;
            if (persistAcrossLoads)
            {
                PlayerPrefs.SetInt(persistenceKey, 1);
                PlayerPrefs.Save();
            }
            ApplySolvedState();
        }
        else
        {
            entered = "";
            status = "ACCESS DENIED";
            statusUntil = Time.unscaledTime + 1.5f;
        }
    }

    public void ClearCode()
    {
        entered = "";
        status = "";
    }

    private void ApplySolvedState()
    {
        solved = true;
        keypadOpen = false;
        foreach (GameObject go in unlockOnSuccess)
            if (go != null) go.SetActive(false);
        foreach (GameObject go in enableOnSuccess)
            if (go != null) go.SetActive(true);
    }

    private void OnGUI()
    {
        if (solved && Time.unscaledTime > statusUntil && string.IsNullOrEmpty(status))
            return;

        if (solved && Time.unscaledTime > statusUntil)
            status = "";

        if (!keypadOpen && !string.IsNullOrEmpty(status))
        {
            DrawStatus(status);
            return;
        }

        if (!keypadOpen || !playerInside) return;

        EnsureStyles();

        float width = Mathf.Min(420f, Screen.width * 0.70f);
        float height = 430f;
        Rect panel = new Rect(
            (Screen.width - width) * 0.5f,
            (Screen.height - height) * 0.5f,
            width,
            height);

        GUI.Box(panel, GUIContent.none, panelStyle);
        GUI.Label(new Rect(panel.x + 22f, panel.y + 18f, panel.width - 44f, 34f),
            "RESEARCH STATION 03 // SECURITY", titleStyle);
        GUI.Label(new Rect(panel.x + 22f, panel.y + 58f, panel.width - 44f, 26f),
            "6-DIGIT ACCESS CODE", displayStyle);

        string masked = "";
        for (int i = 0; i < requiredCode.Length; i++)
            masked += i < entered.Length ? entered[i].ToString() : "•";
        GUI.Label(new Rect(panel.x + 22f, panel.y + 88f, panel.width - 44f, 48f), masked, displayStyle);

        float bx = panel.x + 28f;
        float by = panel.y + 150f;
        float bw = 76f;
        float bh = 48f;
        float gap = 8f;

        string[] keys = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "CLEAR", "0", "ENTER" };
        for (int i = 0; i < keys.Length; i++)
        {
            int row = i / 3;
            int col = i % 3;
            Rect r = new Rect(
                bx + col * (bw + gap),
                by + row * (bh + gap),
                bw,
                bh);

            if (GUI.Button(r, keys[i], buttonStyle))
            {
                string key = keys[i];
                if (key == "CLEAR") ClearCode();
                else if (key == "ENTER") SubmitCode();
                else if (key == "0" || key == "1" || key == "2" || key == "3" || key == "4" ||
                         key == "5" || key == "6" || key == "7" || key == "8" || key == "9")
                {
                    if (entered.Length < requiredCode.Length)
                        entered += key;
                }
            }
        }

        GUI.Label(new Rect(panel.x + 22f, panel.y + 372f, panel.width - 44f, 30f),
            "The access sequence was reconstructed from the field clues in Rung Vong.",
            displayStyle);
    }

    private void DrawStatus(string text)
    {
        EnsureStyles();
        float width = Mathf.Min(360f, Screen.width * 0.62f);
        float height = 58f;
        Rect r = new Rect(
            (Screen.width - width) * 0.5f,
            Screen.height - height - 32f,
            width,
            height);
        GUI.Box(r, text, titleStyle);
    }

    private void EnsureStyles()
    {
        if (panelStyle != null) return;

        panelStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = 16
        };
        titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };
        displayStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
            wordWrap = true
        };
        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 15
        };
    }

    private bool IsPlayer(Collider2D col)
    {
        if (col == null) return false;
        if (col.CompareTag(playerTag)) return true;
        return col.attachedRigidbody != null && col.attachedRigidbody.CompareTag(playerTag);
    }
}
