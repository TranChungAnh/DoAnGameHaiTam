using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerInputHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject mainMenu;

    [Header("Settings")]
    [SerializeField] private float inputHoldTime = 0.2f;

    // --- C# Generated Class Instance ---
    private PlayerInputAction input;

    // --- Components ---
    private PlayerController playerController;
    private ToolManager toolManager;
    private InteractionSystem interactionSystem;
    private Camera cam;

    // --- Input Properties ---
    public Vector2 rawMovementInput { get; private set; }
    public Vector2 rawDashDirectionInput { get; private set; }
    public Vector2Int dashDirectionInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }

    public bool grabInput { get; private set; }
    public bool dashInput { get; private set; }
    public bool dashInputStop { get; private set; }

    public float dashInputStartTime { get; private set; }
    public bool[] attackInput { get; private set; }

    private void Awake()
    {
        try
        {
            input = new PlayerInputAction();
        }
        catch (Exception)
        {
            // Suppressed log
        }

        playerController = GetComponent<PlayerController>();
        toolManager = GetComponent<ToolManager>();
        interactionSystem = GetComponent<InteractionSystem>();
    }

    private void Start()
    {
        cam = Camera.main;
        int count = Enum.GetValues(typeof(CombatInputs)).Length;
        attackInput = new bool[count];

        if (mainMenu == null)
        {
            Transform found = GameObject.Find("BG")?.transform.Find("MainMenu");
            if (found != null) mainMenu = found.gameObject;
        }
    }

    private void OnEnable()
    {
        if (input == null) return;

        input.Gameplay.Enable();

        InputAction moveAction = input.Gameplay.Get().FindAction("Move") ?? input.Gameplay.Get().FindAction("WASD");

        if (moveAction != null)
        {
            moveAction.performed += OnMoveInput;
            moveAction.canceled += OnMoveInput;
        }

        // --- ĐĂNG KÝ CẢM BIẾN TẤN CÔNG ---
        InputAction primaryAction = input.Gameplay.Get().FindAction("PrimaryAttack") ?? input.Gameplay.Get().FindAction("Attack");
        if (primaryAction != null)
        {
            primaryAction.started += OnPrimaryAttack;
            primaryAction.canceled += OnPrimaryAttack;
        }

        //if (input.Gameplay.Get().Contains(input.Gameplay.Dash))
        //{
        //    input.Gameplay.Dash.started += OnDashInput;
        //    input.Gameplay.Dash.canceled += OnDashInput;
        //}

        if (input.Gameplay.Get().Contains(input.Gameplay.UseTool))
            input.Gameplay.UseTool.performed += OnUseTool;

        if (input.Gameplay.Get().Contains(input.Gameplay.PlantSeed))
            input.Gameplay.PlantSeed.performed += OnPlantSeed;

        if (input.Gameplay.Get().Contains(input.Gameplay.Water))
            input.Gameplay.Water.performed += OnWater;

        if (input.Gameplay.Get().Contains(input.Gameplay.Interact))
            input.Gameplay.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (input == null) return;

        InputAction moveAction = input.Gameplay.Get().FindAction("Move") ?? input.Gameplay.Get().FindAction("WASD");
        if (moveAction != null)
        {
            moveAction.performed -= OnMoveInput;
            moveAction.canceled -= OnMoveInput;
        }

        // --- HỦY ĐĂNG KÝ CẢM BIẾN TẤN CÔNG ---
        InputAction primaryAction = input.Gameplay.Get().FindAction("PrimaryAttack") ?? input.Gameplay.Get().FindAction("Attack");
        if (primaryAction != null)
        {
            primaryAction.started -= OnPrimaryAttack;
            primaryAction.canceled -= OnPrimaryAttack;
        }

        //if (input.Gameplay.Get().Contains(input.Gameplay.Dash))
        //{
        //    input.Gameplay.Dash.started -= OnDashInput;
        //    input.Gameplay.Dash.canceled -= OnDashInput;
        //}

        if (input.Gameplay.Get().Contains(input.Gameplay.UseTool))
            input.Gameplay.UseTool.performed -= OnUseTool;

        if (input.Gameplay.Get().Contains(input.Gameplay.PlantSeed))
            input.Gameplay.PlantSeed.performed -= OnPlantSeed;

        if (input.Gameplay.Get().Contains(input.Gameplay.Water))
            input.Gameplay.Water.performed -= OnWater;

        if (input.Gameplay.Get().Contains(input.Gameplay.Interact))
            input.Gameplay.Interact.performed -= OnInteract;

        input.Gameplay.Disable();
    }

    private void Update()
    {
        CheckDashInputHoldTime();
    }

    // ---------------- LOGIC DI CHUYỂN & INPUT ----------------

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        rawMovementInput = context.ReadValue<Vector2>();
        NormInputX = Mathf.RoundToInt(rawMovementInput.x);
        NormInputY = Mathf.RoundToInt(rawMovementInput.y);

        if (playerController != null)
        {
            playerController.OnMove(rawMovementInput);
        }
    }

    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            dashInput = true;
            dashInputStop = false;
            dashInputStartTime = Time.time;
        }
        else if (context.canceled)
        {
            dashInputStop = true;
        }
    }

    // ---------------- LOGIC TẤN CÔNG ----------------

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {

            // Cập nhật biến cờ nếu cần
            if (attackInput != null && attackInput.Length > 0)
            {
                attackInput[(int)CombatInputs.primary] = true;
            }

            // Gọi trực tiếp State Machine đổi sang Primary Attack State
            if (playerController != null)
            {
               
                var playerScript = GetComponent<Player>();
                if (playerScript != null && playerScript.stateMachine != null)
                {
                    playerScript.stateMachine.ChangeState(playerScript.primaryAttackState);
                }
            }
        }
        else if (context.canceled)
        {
            if (attackInput != null && attackInput.Length > 0)
            {
                attackInput[(int)CombatInputs.primary] = false;
            }
        }
    }
    public void UseAttackInput(int i) => attackInput[i] = false;

    // --- Farming & Interaction Handlers ---

    private void OnUseTool(InputAction.CallbackContext context)
    {
        if (context.performed && toolManager != null)
        {
            toolManager.UseTool();
        }
    }

    private void OnPlantSeed(InputAction.CallbackContext context)
    {
        if (context.performed && toolManager != null)
        {
            toolManager.PlantSeedPublic();
        }
    }

    private void OnWater(InputAction.CallbackContext context)
    {
        if (context.performed && toolManager != null)
        {
            toolManager.WaterPublic();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && interactionSystem != null)
        {
            interactionSystem.TryClickInteract();
        }
    }

    // ---------------- HELPER METHODS ----------------

    public void UseDashInput() => dashInput = false;

    private void CheckDashInputHoldTime()
    {
        if (Time.time >= dashInputStartTime + inputHoldTime)
        {
            dashInput = false;
        }
    }

    public enum CombatInputs
    {
        primary,
        secondary
    }
}