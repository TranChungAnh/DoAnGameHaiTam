using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Lightweight narrative/flashback trigger for Research Station 03.
/// It can show a temporary on-screen message and toggle memory visuals.
/// It does not depend on the project's dialogue system.
/// </summary>
public class ResearchStationNarrativeTrigger : MonoBehaviour
{
    [TextArea(2, 6)]
    public string message;

    public bool triggerOnce = true;
    public bool showMessageOnScreen = true;
    public float messageDuration = 3.0f;

    [Header("Memory Visuals")]
    public GameObject[] enableOnTrigger;
    public GameObject[] disableOnTrigger;
    public float visualDuration = 2.5f;

    [Header("Optional")]
    public AudioClip cue;
    public UnityEvent onTriggered;

    private bool triggered;
    private GUIStyle style;
    private float messageUntil;
    private string activeMessage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && triggered) return;
        if (!IsPlayer(other)) return;

        triggered = true;
        ActivateNarrative();
    }

    public void ActivateNarrative()
    {
        activeMessage = message;
        messageUntil = Time.unscaledTime + messageDuration;

        foreach (GameObject go in enableOnTrigger)
            if (go != null) go.SetActive(true);

        foreach (GameObject go in disableOnTrigger)
            if (go != null) go.SetActive(false);

        if (cue != null)
            AudioSource.PlayClipAtPoint(cue, transform.position);

        if (!string.IsNullOrWhiteSpace(message))
            Debug.Log("[HẢI TÂM][RESEARCH STATION] " + message);

        onTriggered?.Invoke();

        if (visualDuration > 0f && enableOnTrigger != null && enableOnTrigger.Length > 0)
            StartCoroutine(DisableTemporaryVisuals());
    }

    private IEnumerator DisableTemporaryVisuals()
    {
        yield return new WaitForSeconds(visualDuration);

        foreach (GameObject go in enableOnTrigger)
            if (go != null) go.SetActive(false);

        foreach (GameObject go in disableOnTrigger)
            if (go != null) go.SetActive(true);
    }

    private void OnGUI()
    {
        if (!showMessageOnScreen || string.IsNullOrEmpty(activeMessage)) return;
        if (Time.unscaledTime > messageUntil) return;

        style ??= new GUIStyle(GUI.skin.box)
        {
            fontSize = Mathf.Max(14, Screen.height / 40),
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        float width = Mathf.Min(Screen.width * 0.76f, 900f);
        float height = Mathf.Min(Screen.height * 0.20f, 150f);
        Rect rect = new Rect(
            (Screen.width - width) * 0.5f,
            Screen.height - height - 30f,
            width,
            height);

        GUI.Box(rect, activeMessage, style);
    }

    private static bool IsPlayer(Collider2D col)
    {
        if (col == null) return false;
        if (col.CompareTag("Player")) return true;
        if (col.attachedRigidbody != null && col.attachedRigidbody.CompareTag("Player")) return true;
        return false;
    }
}
