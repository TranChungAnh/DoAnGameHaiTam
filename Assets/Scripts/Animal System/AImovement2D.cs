using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AImovement2D : MonoBehaviour
{
    private Vector2 direction;
    private float stateTimer;
    private float speed = 1f;

    private Animator animator;

    private enum State { Idle, Walk }
    private State currentState = State.Idle;

    private Transform homeTarget;
    private bool isGoingHome = false;
    private Coroutine goHomeRoutine;
    private Animal animal;
    // Raycast config
    [SerializeField] private float rayDistance = 0.5f;
    [SerializeField] private LayerMask obstacleLayer;

    // 🔹 Idle & Walk random range
    [Header("Idle Settings")]
    [SerializeField] private float idleMin = 2f;
    [SerializeField] private float idleMax = 5f;

    [Header("Walk Settings")]
    [SerializeField] private float walkMin = 3f;
    [SerializeField] private float walkMax = 6f;


    // 4 hướng cố định
    private readonly Vector2[] fourDirections = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    void Start()
    {
        animal = GetComponent<Animal>();
        speed = GetComponent<Animal>().data.moveSpeed;
        animator = GetComponent<Animator>();

        SetState(State.Idle);
    }
    private void OnEnable()
    {
        TimeManager.OnNightStarted += GoHomeAtNight;
        TimeManager.OnDayStarted += GoOutInMorning;
    }
    private void OnDisable()
    {
        TimeManager.OnNightStarted -= GoHomeAtNight;
        TimeManager.OnDayStarted -= GoOutInMorning;
    }
    private void GoHomeAtNight()
    {
        string tag = animal.data.homeTag; // 🔸 lấy tag từ AnimalData
        Debug.Log($"{name} nghe sự kiện -> trời tối, đi về chuồng có tag {tag}");
        GoHome(tag);
    }

    private void GoOutInMorning()
    {
        Debug.Log($"{name} nghe sự kiện -> trời sáng, bắt đầu đi dạo");
        SetState(State.Walk);
    }
    void Update()
    {
        if (isGoingHome) return;
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            if (currentState == State.Idle)
                SetState(State.Walk);
            else
                SetState(State.Idle);
        }

        if (currentState == State.Walk && direction != Vector2.zero)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayDistance, obstacleLayer);

            if (hit.collider != null)
            {
                Debug.Log($"{name} gặp vật cản: {hit.collider.name}");
                SetState(State.Idle); // dừng ngay lập tức
            }
            else
            {
                transform.Translate(direction * speed * Time.deltaTime);
            }
        }

        UpdateAnimation();
    }

    public void GoHome(string homeTag)
    {
        GameObject home=GameObject.FindGameObjectWithTag(homeTag);
        if (home == null)
        {
            Debug.Log($"{name} không tìm thấy chuồng có tag {homeTag}!");
            return;
        }
        homeTarget = home.transform;
        if (goHomeRoutine != null)
        {
            StopCoroutine(goHomeRoutine);
        }
        goHomeRoutine=StartCoroutine(MoveToHomeCoroutine());
    }
    private IEnumerator MoveToHomeCoroutine()
    {
        isGoingHome = true;
        animator.SetBool("IsWalking", true);
        while (homeTarget != null && Vector2.Distance(transform.position, homeTarget.position) > 0.3f)
        {
            Vector2 dir = (homeTarget.position - transform.position).normalized;
            direction = dir;
            UpdateAnimation();
            transform.position = Vector2.MoveTowards(transform.position, homeTarget.position, speed * Time.deltaTime);
            UpdateAnimation();

            yield return null; 
        }
        animator.SetBool("IsWalking", false);
        isGoingHome = false;
        //animator.SetTrigger("Sleep");
        goHomeRoutine = null;
    }
   
    private void SetState(State newState)
    {
        currentState = newState;

        if (newState == State.Idle)
        {
            // 🔹 Random thời gian idle
            stateTimer = Random.Range(idleMin, idleMax);
            animator.SetBool("IsWalking", false);
        }
        else if (newState == State.Walk)
        {
            // 🔹 Random thời gian walk
            stateTimer = Random.Range(walkMin, walkMax);
            direction = fourDirections[Random.Range(0, fourDirections.Length)];
            animator.SetBool("IsWalking", true);
        }
    }

    private void UpdateAnimation()
    {
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
    }

    public void PlayProduceAnimation()
    {
        animator.SetTrigger("Produce");
    }

    private void OnDrawGizmos()
    {
        if (currentState == State.Walk && direction != Vector2.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + direction.normalized * rayDistance);
            Gizmos.DrawSphere((Vector2)transform.position + direction.normalized * rayDistance, 0.05f);
        }
    }
}
