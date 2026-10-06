using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AImovement2D), typeof(Animator))]
public class Animal : MonoBehaviour, IProduce
{
    [Header("Data")]
    public AnimalData data;

    private AImovement2D ai;
    private Animator anim;

    // Runtime state
    private readonly Dictionary<AnimalStateType, float> stateValues = new();
    private readonly Dictionary<AnimalStateType, bool> stateActives = new();
    private readonly Dictionary<AnimalStateType, float> stateTimers = new();
    private readonly Dictionary<AnimalStateType, GameObject> stateIcons = new();

    private float produceTimer;
    private float age = 0f;
    private bool isAdult = false;

    public bool IsDead { get; private set; } = false;
    public bool isAlive = true;

    void Awake()
    {
        ai = GetComponent<AImovement2D>();
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = data.animatorController;

        // Clone để runtime có bản riêng
        data = Instantiate(data);

        // Random cho state
        for (int i = 0; i < data.states.Count; i++)
        {
            var s = data.states[i];
            float randomRate = Random.Range(s.minRate, s.maxRate);
            data.states[i].rate = randomRate;

            stateValues[s.stateType] = 0f;
            stateActives[s.stateType] = false;
            stateTimers[s.stateType] = 0f;

            if (s.iconPrefab != null)
            {
                var icon = Instantiate(s.iconPrefab, transform);
                icon.SetActive(false);
                stateIcons[s.stateType] = icon;
            }
        }

        for (int i = 0; i < data.produces.Count; i++)
        {
            var p = data.produces[i];

            if (System.Enum.TryParse(p.produceName, true, out AnimalStateType stateType))
            {
                var state = data.states.Find(st => st.stateType == stateType);
                if (state != null)
                    data.produces[i].cooldown = state.rate;
            }
            else
            {
                float randomCooldown = Random.Range(p.minCooldown, p.maxCooldown);
                data.produces[i].cooldown = randomCooldown;
            }
        }

        transform.localScale = Vector3.one * data.minScale;
    }


    void Start()
    {
        AnimalManager.Instance?.RegisterAnimal(this);
    }

    void OnDestroy()
    {
        AnimalManager.Instance?.UnregisterAnimal(this);
    }

    void Update()
    {
        if (IsDead) return;

        UpdateAgeAndGrowth();
        UpdateStates();
        UpdateProduction();
    }

    // =========================================================
    //  Age & Growth
    // =========================================================
    private void UpdateAgeAndGrowth()
    {
        age += Time.deltaTime;

        if (!isAdult)
        {
            float t = Mathf.Clamp01(age / data.growDuration);
            float scale = Mathf.Lerp(data.minScale, data.maxScale, t);
            transform.localScale = Vector3.one * scale;

            if (age >= data.growDuration)
            {
                isAdult = true;
                Debug.Log($"{data.animalName} đã trưởng thành!");
            }
        }

        if (age >= data.lifeDuration)
            Die(AnimalStateType.Sick); // Chết vì già
    }

    // =========================================================
    // State Handling (Hunger, Thirst, Sickness…)
    // =========================================================
    private void UpdateStates()
    {
        foreach (var s in data.states)
        {
            stateValues[s.stateType] += Time.deltaTime / s.rate;

            if (stateValues[s.stateType] >= 1f && !stateActives[s.stateType])
                ActivateState(s);

            if (stateActives[s.stateType])
            {
                stateTimers[s.stateType] -= Time.deltaTime;
                if (stateTimers[s.stateType] <= 0f)
                {
                    Die(s.stateType);
                    return;
                }
            }
        }
    }

    private void ActivateState(AnimalStateData s)
    {
        stateActives[s.stateType] = true;
        stateTimers[s.stateType] = s.surviveDuration;

        if (stateIcons.ContainsKey(s.stateType))
            stateIcons[s.stateType].SetActive(true);

    }

    public void CureState(AnimalStateType type)
    {
        stateValues[type] = 0f;
        stateActives[type] = false;
        stateTimers[type] = 0f;

        if (stateIcons.ContainsKey(type))
            stateIcons[type].SetActive(false);

        Debug.Log($"{data.animalName} đã được chữa {type}!");
    }

    public bool IsStateActive(AnimalStateType type) =>
        stateActives.ContainsKey(type) && stateActives[type];

    // =========================================================
    //  Death
    // =========================================================
    private void Die(AnimalStateType cause)
    {
        if (IsDead) return;
        IsDead = true;
        isAlive = false;

        ai.enabled = false;

        anim?.SetTrigger("Die");
        Destroy(gameObject, 3f);
    }

    // =========================================================
    //  Production
    // =========================================================
    private void UpdateProduction()
    {
        if (!isAdult) return;

        produceTimer += Time.deltaTime;
        if (CanProduce())
        {
            ai.PlayProduceAnimation();
            Produce();
        }
    }

    public bool CanProduce()
    {
        if (!isAlive || !isAdult) return false;

        foreach (var produce in data.produces)
            if (produceTimer >= produce.cooldown)
                return true;

        return false;
    }

    public void Produce()
    {
        if (!isAlive || !isAdult) return;

        produceTimer = 0f; // reset

        foreach (var p in data.produces)
        {
            if (p.type == ProduceType.Harvest)
            {
                if (System.Enum.TryParse(p.produceName, out AnimalStateType stateType))
                {
                    if (!IsStateActive(stateType))
                        ActivateState(new AnimalStateData { stateType = stateType, surviveDuration = 9999f });
                }
            }
            else
            {
                HarvestProduce(p); 
            }
        }
    }

    public void HarvestProduce(ProduceData produce)
    {
        var storage = InventoryManager.Instance.playerInventory;

        if (!string.IsNullOrEmpty(produce.requiredTool) && !storage.HasItem(produce.requiredTool))
        {
            Debug.Log($" Cần {produce.requiredTool} để thu hoạch {produce.produceName}!");
            return;
        }

        switch (produce.type)
        {
            case ProduceType.Harvest:
                if (storage.AddItem(produce.produceItem, 1))
                {
                    Debug.Log($"{data.animalName} đã cho {produce.produceName}!");

                    if (System.Enum.TryParse(produce.produceName, out AnimalStateType stateType))
                    {
                        CureState(stateType);
                    }
                }
                else
                {
                    Debug.Log(" Kho đã đầy, không thể thu hoạch!");
                }
                break;

            case ProduceType.Reproduce:
                if (produce.producePrefab != null)
                {
                    Instantiate(produce.producePrefab, transform.position, Quaternion.identity);
                    // 🔹 Sau khi sinh, reset timer hoặc tắt trạng thái
                    if (System.Enum.TryParse(produce.produceName, true, out AnimalStateType stateType))
                    {
                        CureState(stateType); // tắt trạng thái (icon, flag…)
                    }

                    Debug.Log($"{data.animalName} đã sinh ra {produce.produceName}!");
                }
                else
                {
                    Debug.LogWarning($" {data.animalName} có ProduceType.Reproduce nhưng thiếu prefab!");
                }
                break;
        }
    }
}
