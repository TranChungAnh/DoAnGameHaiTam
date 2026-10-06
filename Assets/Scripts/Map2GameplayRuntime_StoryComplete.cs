using UnityEngine;
using UnityEngine.SceneManagement;

public class Map2GameplayController : MonoBehaviour
{
    public string requiredCode = "123456";
    public int requiredDigitCount = 6;
    public int foundDigits { get; private set; }
    public bool largeMonsterDefeated { get; private set; }
    public bool instructionMapCollected { get; private set; }
    public bool codeAccepted { get; private set; }
    public bool powerRestored { get; private set; }
    public bool artifact03Collected { get; private set; }
    public bool artifact04Collected { get; private set; }
    public bool researchFileCollected { get; private set; }
    public bool fatherFlashbackSeen { get; private set; }

    public bool CanFightLargeMonster => foundDigits >= requiredDigitCount;
    public bool CanReadAccessMap => largeMonsterDefeated;
    public bool CanUseStation => codeAccepted && powerRestored;

    public void RegisterDigit(int value)
    {
        foundDigits = Mathf.Clamp(foundDigits + 1, 0, requiredDigitCount);
        Debug.Log("[MAP2] Digit clue collected: " + value + " (" + foundDigits + "/" + requiredDigitCount + ")");
    }

    public void RegisterLargeMonsterDefeated()
    {
        largeMonsterDefeated = true;
        Map2InstructionMapPickup[] maps = FindObjectsOfType<Map2InstructionMapPickup>(true);
        for (int i = 0; i < maps.Length; i++)
            maps[i].Reveal();
        Debug.Log("[MAP2] Large monster defeated. Access-order map dropped.");
    }

    public bool RegisterInstructionMap()
    {
        if (!largeMonsterDefeated)
            return false;
        instructionMapCollected = true;
        Debug.Log("[MAP2] Access-order map collected. Six-digit ordering clue unlocked.");
        return true;
    }

    public void RegisterArtifact04()
    {
        if (artifact04Collected)
            return;
        artifact04Collected = true;
        Debug.Log("[MAP2] Artifact 04 collected from the special forest monster.");
    }

    public bool TryDefeatLargeMonster()
    {
        if (!CanFightLargeMonster)
        {
            Debug.Log("[MAP2] Large monster is locked until all six digit clues are collected.");
            return false;
        }
        if (largeMonsterDefeated)
            return false;
        RegisterLargeMonsterDefeated();
        return true;
    }

    public bool SubmitAccessCode(string code)
    {
        if (!largeMonsterDefeated || !instructionMapCollected)
        {
            Debug.Log("[MAP2] Access code is locked until the large monster and its ordering map are obtained.");
            return false;
        }
        if (string.IsNullOrEmpty(code) || code != requiredCode)
        {
            Debug.Log("[MAP2] Incorrect access code.");
            return false;
        }
        codeAccepted = true;
        Debug.Log("[MAP2] Access code accepted. Research Station door can open after power is restored.");
        return true;
    }

    public void RegisterPowerRestored()
    {
        powerRestored = true;
        Debug.Log("[MAP2] Research Station power restored.");
    }

    public bool CollectArtifact03()
    {
        if (!CanUseStation)
        {
            Debug.Log("[MAP2] Artifact 03 is still locked.");
            return false;
        }
        if (artifact03Collected)
            return false;
        artifact03Collected = true;
        Debug.Log("[MAP2] Artifact 03 collected. Research memory event unlocked.");
        return true;
    }

    public bool CollectResearchFile()
    {
        if (!CanUseStation)
        {
            Debug.Log("[MAP2] The Bột Mộng Thức research file is locked until the station is restored.");
            return false;
        }
        if (researchFileCollected)
            return false;
        researchFileCollected = true;
        Debug.Log("[MAP2] Research file Bột Mộng Thức collected. Memory-loss and reconstruction clue unlocked.");
        return true;
    }

    public bool TriggerFatherFlashback()
    {
        if (!CanUseStation)
            return false;
        if (fatherFlashbackSeen)
            return true;
        fatherFlashbackSeen = true;
        Debug.Log("[MAP2] Day 4 flashback: Minh sees his father working inside Research Station 03.");
        return true;
    }
}

public class Map2DigitClue : MonoBehaviour
{
    public int digitValue = 1;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !Map2PlayerDetector.IsPlayer(other))
            return;
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        if (controller == null)
            return;
        collected = true;
        controller.RegisterDigit(digitValue);
        Collider2D c = GetComponent<Collider2D>();
        if (c != null)
            c.enabled = false;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
    }
}

public class Map2InstructionMapPickup : MonoBehaviour
{
    public int mapId = 2;
    private bool collected;
    private bool revealed;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Reveal()
    {
        revealed = true;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!revealed || collected || !Map2PlayerDetector.IsPlayer(other))
            return;
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        if (controller == null || !controller.RegisterInstructionMap())
            return;
        collected = true;
        Collider2D c = GetComponent<Collider2D>();
        if (c != null)
            c.enabled = false;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
    }
}

public class Map2ArtifactPickup : MonoBehaviour
{
    public int artifactId = 4;
    public bool requiresStationAccess;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !Map2PlayerDetector.IsPlayer(other))
            return;
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        if (controller == null)
            return;
        bool accepted;
        if (artifactId == 4)
        {
            accepted = !controller.artifact04Collected;
            if (accepted)
                controller.RegisterArtifact04();
        }
        else
        {
            accepted = !requiresStationAccess || controller.CanUseStation;
            if (accepted)
                accepted = controller.CollectArtifact03();
        }
        if (!accepted)
            return;
        collected = true;
        Collider2D c = GetComponent<Collider2D>();
        if (c != null)
            c.enabled = false;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
    }
}

public class Map2SpecialMonster : MonoBehaviour
{
    public GameObject artifactDrop;
    public bool defeated { get; private set; }
    private Map2GameplayController controller;

    private void Awake()
    {
        controller = FindObjectOfType<Map2GameplayController>();
    }

    public bool Defeat()
    {
        if (defeated || controller == null)
            return false;
        defeated = true;
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (artifactDrop == null || colliders[i].gameObject != artifactDrop)
                colliders[i].enabled = false;
        }
        Transform visual = transform.Find("SpecialSmallMonsterVisual");
        if (visual != null)
            visual.gameObject.SetActive(false);
        if (artifactDrop != null)
            artifactDrop.SetActive(true);
        Debug.Log("[MAP2] Special small monster defeated. Artifact 04 dropped.");
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!defeated && Map2PlayerDetector.IsPlayer(other))
            Defeat();
    }
}

public class Map2LargeMonster : MonoBehaviour
{
    public bool defeated { get; private set; }
    public GameObject instructionMapDrop;
    private Map2GameplayController controller;

    private void Awake()
    {
        controller = FindObjectOfType<Map2GameplayController>();
    }

    public bool Defeat()
    {
        if (defeated || controller == null)
            return false;
        if (!controller.TryDefeatLargeMonster())
            return false;
        defeated = true;
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (instructionMapDrop == null || colliders[i].gameObject != instructionMapDrop)
                colliders[i].enabled = false;
        }
        Transform visual = transform.Find("LargeMonster");
        if (visual != null)
            visual.gameObject.SetActive(false);
        if (instructionMapDrop != null)
            instructionMapDrop.SetActive(true);
        return true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!defeated && Map2PlayerDetector.IsPlayer(other))
            Defeat();
    }
}

public class Map2ResearchFilePickup : MonoBehaviour
{
    public string fileId = "Bột Mộng Thức";
    public bool requiresStationAccess = true;
    private bool collected;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !Map2PlayerDetector.IsPlayer(other))
            return;
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        if (controller == null)
            return;
        if (requiresStationAccess && !controller.CanUseStation)
            return;
        if (!controller.CollectResearchFile())
            return;
        collected = true;
        Collider2D c = GetComponent<Collider2D>();
        if (c != null)
            c.enabled = false;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
        Debug.Log("[MAP2] Collected research file: " + fileId);
    }
}

public class Map2FatherFlashback : MonoBehaviour
{
    public GameObject flashbackVisual;
    private bool triggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered || !Map2PlayerDetector.IsPlayer(other))
            return;
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        if (controller == null || !controller.TriggerFatherFlashback())
            return;
        triggered = true;
        if (flashbackVisual != null)
            flashbackVisual.SetActive(true);
        Debug.Log("[MAP2] Flashback activated: Minh sees his father at work in Research Station 03.");
    }
}

public class Map2SwitchPuzzle : MonoBehaviour
{
    public readonly bool[] states = new bool[6];
    public bool solved { get; private set; }
    private Map2GameplayController controller;

    private void Awake()
    {
        controller = FindObjectOfType<Map2GameplayController>();
    }

    public void Toggle(int index)
    {
        if (solved || index < 0 || index >= 6)
            return;
        ToggleSingle(index);
        if (index >= 3)
            ToggleSingle(index - 3);
        else
            ToggleSingle(index + 3);
        if (index % 3 > 0)
            ToggleSingle(index - 1);
        if (index % 3 < 2)
            ToggleSingle(index + 1);
        CheckSolved();
    }

    private void ToggleSingle(int index)
    {
        states[index] = !states[index];
        Transform sw = transform.Find("Switch_" + (index + 1));
        if (sw == null)
            return;
        SpriteRenderer sr = sw.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.transform.localScale = states[index] ? Vector3.one * 1.18f : Vector3.one;
    }

    private void CheckSolved()
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (!states[i])
                return;
        }
        solved = true;
        if (controller == null)
            controller = FindObjectOfType<Map2GameplayController>();
        if (controller != null)
            controller.RegisterPowerRestored();
        Debug.Log("[MAP2] Six-switch power puzzle solved.");
    }
}

public class Map2Switch : MonoBehaviour
{
    public int index;
    public Map2SwitchPuzzle puzzle;
    private bool locked;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (locked || puzzle == null || !Map2PlayerDetector.IsPlayer(other))
            return;
        locked = true;
        puzzle.Toggle(index);
        Invoke(nameof(Unlock), 0.25f);
    }

    private void Unlock()
    {
        locked = false;
    }
}

public class Map2CodeTerminal : MonoBehaviour
{
    public string requiredCode = "123456";

    public bool SubmitCode(string code)
    {
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        return controller != null && controller.SubmitAccessCode(code);
    }
}

public class Map2StationDoor : MonoBehaviour
{
    public string doorVisualName = "StationMainDoor";
    private bool opened;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (opened || !Map2PlayerDetector.IsPlayer(other))
            return;
        Map2GameplayController controller = FindObjectOfType<Map2GameplayController>();
        if (controller == null || !controller.CanUseStation)
        {
            Debug.Log("[MAP2] Research Station requires the access code and restored power.");
            return;
        }
        OpenDoor();
    }

    public void OpenDoor()
    {
        if (opened)
            return;
        opened = true;
        Collider2D trigger = GetComponent<Collider2D>();
        if (trigger != null)
            trigger.enabled = false;
        Transform station = transform.parent;
        if (station != null)
        {
            Transform visual = station.Find(doorVisualName);
            if (visual != null)
                visual.gameObject.SetActive(false);
        }
        Debug.Log("[MAP2] Research Station door opened.");
    }
}

public class Map2TransitionTrigger : MonoBehaviour
{
    public string targetScene;
    public string direction;
    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !Map2PlayerDetector.IsPlayer(other) || string.IsNullOrEmpty(targetScene))
            return;
        used = true;
        if (Application.CanStreamedLevelBeLoaded(targetScene))
            SceneManager.LoadScene(targetScene);
        else
            Debug.Log("[MAP2] Transition ready: " + direction + " -> " + targetScene);
    }
}

public static class Map2PlayerDetector
{
    public static bool IsPlayer(Collider2D other)
    {
        if (other == null)
            return false;
        Transform t = other.transform;
        if (t.CompareTag("Player"))
            return true;
        Transform parent = t.parent;
        if (parent != null && parent.CompareTag("Player"))
            return true;
        string a = t.name.ToLowerInvariant();
        string b = parent != null ? parent.name.ToLowerInvariant() : string.Empty;
        return a.Contains("player") || b.Contains("player");
    }
}
