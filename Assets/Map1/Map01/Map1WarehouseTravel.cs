using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keep Map01 (Player, Inventory, camera, managers) loaded while visiting
/// the warehouse interior additively. Attach to a standalone, always-active
/// root GameObject in Map01, NOT to an exterior object that will be hidden.
/// </summary>
public class Map1WarehouseTravel : MonoBehaviour
{
    public static Map1WarehouseTravel Instance { get; private set; }

    [Header("Scene noi that")]
    [SerializeField] private string interiorSceneName = "Map1_WarehouseInterior";
    [SerializeField] private string interiorEntryMarker = "WarehouseEntryPoint";

    [Header("Map01 - chi gan cac doi tuong ngoai troi")]
    [SerializeField] private GameObject[] exteriorRoots;

    [Header("Tham chieu Map01")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform outsideReturnPoint;

    private readonly List<GameObject> hiddenObjects = new List<GameObject>();
    private Scene exteriorScene;
    private Scene interiorScene;
    private bool transitioning;
    private bool inside;

    public bool IsTransitioning => transitioning;
    public bool IsInside => inside;
    public bool CanEnterWarehouse => Application.CanStreamedLevelBeLoaded(interiorSceneName);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("[Map1] Chi duoc co mot Map1WarehouseTravel trong Map01.", this);
            enabled = false;
            return;
        }

        Instance = this;
        exteriorScene = gameObject.scene;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private bool CheckReferences()
    {
        if (player == null)
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            if (controller != null) player = controller.transform;
        }

        if (player == null || outsideReturnPoint == null)
        {
            Debug.LogError("[Map1] Can gan Player va Outside Return Point tren Map1WarehouseTravel.", this);
            return false;
        }
        return true;
    }

    public bool EnterWarehouse()
    {
        if (transitioning || inside) return false;
        if (!CheckReferences()) return false;
        if (!CanEnterWarehouse)
        {
            Debug.LogError("[Map1] Chua them Scene '" + interiorSceneName + "' vao Build Settings.", this);
            return false;
        }

        StartCoroutine(EnterRoutine());
        return true;
    }

    private IEnumerator EnterRoutine()
    {
        transitioning = true;
        SetMovementEnabled(false);

        AsyncOperation load = SceneManager.LoadSceneAsync(interiorSceneName, LoadSceneMode.Additive);
        if (load == null)
        {
            Debug.LogError("[Map1] Khong the tai Scene noi that.", this);
            SetMovementEnabled(true);
            transitioning = false;
            yield break;
        }
        yield return load;

        interiorScene = SceneManager.GetSceneByName(interiorSceneName);
        Transform marker = FindMarkerInScene(interiorScene, interiorEntryMarker);
        if (!interiorScene.IsValid() || !interiorScene.isLoaded || marker == null)
        {
            Debug.LogError("[Map1] Khong tim thay WarehouseEntryPoint trong Scene noi that.", this);
            if (interiorScene.IsValid() && interiorScene.isLoaded)
                yield return SceneManager.UnloadSceneAsync(interiorScene);
            SetMovementEnabled(true);
            transitioning = false;
            yield break;
        }

        HideExterior();
        TeleportPlayer(marker.position);
        SceneManager.SetActiveScene(interiorScene);
        inside = true;
        SetMovementEnabled(true);
        transitioning = false;
        Debug.Log("[Map1] Da vao nha kho.");
    }

    public bool ExitWarehouse()
    {
        if (transitioning || !inside) return false;
        if (!CheckReferences()) return false;
        StartCoroutine(ExitRoutine());
        return true;
    }

    private IEnumerator ExitRoutine()
    {
        transitioning = true;
        SetMovementEnabled(false);

        // Reactivate Map01 and put Minh in front of the warehouse entrance.
        RestoreExterior();
        SceneManager.SetActiveScene(exteriorScene);
        TeleportPlayer(outsideReturnPoint.position);
        inside = false;

        if (interiorScene.IsValid() && interiorScene.isLoaded)
            yield return SceneManager.UnloadSceneAsync(interiorScene);

        SetMovementEnabled(true);
        transitioning = false;
        Debug.Log("[Map1] Da tro lai Map01.");
    }

    private void HideExterior()
    {
        hiddenObjects.Clear();
        if (exteriorRoots == null) return;

        foreach (GameObject target in exteriorRoots)
        {
            if (target == null || !target.activeSelf) continue;
            // Never deactivate this controller or the persistent Player.
            if (transform.IsChildOf(target.transform) || player.IsChildOf(target.transform))
            {
                Debug.LogWarning("[Map1] Khong an root chua Player hoac Travel: " + target.name, target);
                continue;
            }

            target.SetActive(false);
            hiddenObjects.Add(target);
        }
    }

    private void RestoreExterior()
    {
        foreach (GameObject obj in hiddenObjects)
            if (obj != null) obj.SetActive(true);
        hiddenObjects.Clear();
    }

    private void TeleportPlayer(Vector3 position)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.position = new Vector2(position.x, position.y);
        }
        player.position = new Vector3(position.x, position.y, player.position.z);
        Physics2D.SyncTransforms();
    }

    private void SetMovementEnabled(bool allowMovement)
    {
        if (player == null) return;
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null) controller.canMove = allowMovement;
        if (!allowMovement)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    private static Transform FindMarkerInScene(Scene scene, string markerName)
    {
        if (!scene.IsValid() || !scene.isLoaded) return null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            Transform[] descendants = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform item in descendants)
                if (item.name == markerName) return item;
        }
        return null;
    }
}
