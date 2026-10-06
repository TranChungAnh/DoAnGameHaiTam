using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    private enum  State
    {
     Moving,
     KnockBack,
     Dead 
    }
    private State currentState;

    [SerializeField]
    private float GroundCheckDistance, wallCheckDistance, movementSpeed,maxHealth,knockBackDuration,
        lastTouchDamageTime,touchDamageCooldown,touchDamage,touchDamageWidth,touchDamageHeight;
    private float currenHealth,knockBackStartTime;
    [SerializeField]
    private Vector2 knockBackSpeed;
    [SerializeField]
    private Transform groundCheck, wallCheck,touchDamageCheck;
    [SerializeField]
    private LayerMask whatIsGround,whatIsPlayer;
    [SerializeField]
    private bool groundDetected, walldetected;// phát hiện mặt đất 
    private int facingDirection,DamageDirection;// hướng 
    private GameObject alive;
    private Vector2 movement,
        TouchDamageBottLeft ,TouchDamageTopRight;// vùng va chạm 
    //private AttackDetails attackDetails;
    private Rigidbody2D aliverb;
    [SerializeField]
    private GameObject hitParticle, deathChunkParticle, deathBloodparticle;// hạt , mảnh vụn khi chết và máy bắn 
    private Animator aliveAnim;

    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliverb = alive.GetComponent<Rigidbody2D>();
        facingDirection = 1;
        currenHealth = maxHealth;
        aliveAnim = alive.GetComponent<Animator>();
    }
    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.KnockBack:
                UpdateKnockBackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }







    // ---- WALLKING STATE ----------------

    private void EnterMoveingState()
    {

    }
    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, GroundCheckDistance, whatIsGround);
        walldetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);
        CheckTouchDamage();
    if (!groundDetected || walldetected)
        {
            Flip();
        }
        else
        {
            movement.Set(movementSpeed * facingDirection, aliverb.velocity.y);
            aliverb.velocity = movement;
        }
    
    
    }

    private void ExitMovingState()
    {

    }

    // ----- KNOCKBACK STATE --------------

    private void EnterKnockBackState()
    {
        knockBackStartTime = Time.time;
        movement.Set(knockBackSpeed.x * DamageDirection, knockBackSpeed.y);
        aliverb.velocity = movement;
        aliveAnim.SetBool("knockBack",true);
    }
    private void UpdateKnockBackState()
    {
        if(Time.time>= knockBackStartTime+ knockBackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockBackState()
    {
        aliveAnim.SetBool("knockBack", false);
    }
    // ----- DEAD STATE --------------

    private void EnterDeadState()
    {
        Instantiate(deathChunkParticle, alive.transform.position, deathChunkParticle.transform.rotation);
        Instantiate(deathBloodparticle, alive.transform.position, deathBloodparticle.transform.rotation);
        Destroy(gameObject);
    }
    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    // ------ CÁC HÀM KHÁC ---------------\

    // attackdetails[0] : là lượng sát thương , attackdetails[1]: vị trí X của kể tấn công 
    //private void Damage1(AttackDetails attackDetails)
    //{
    //    currenHealth -= attackDetails.damageAmount;
    //    Instantiate(hitParticle, alive.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f,360.0f)));
    //    // nếu vt kẻ tấn công bên phải enemy 
    //    if (attackDetails.position.x > alive.transform.position.x)
    //    {
    //        DamageDirection = -1;
    //    }
    //    else
    //    {
    //        DamageDirection = 1;
    //    }

    //    // va chạm hạt máu 
    //    if (currenHealth > 0.0f)
    //    {
    //        SwitchState(State.KnockBack);
    //    }
    //    else if(currenHealth <= 0.0f)
    //    {
    //        SwitchState(State.Dead);
    //    }
    //}







    // lật 
    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);
    }



    // chuyển trạng thái 
    private void SwitchState(State state)
    {
        // thoát khỏi trạng thái hiện tại 
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.KnockBack:
                ExitKnockBackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }
        // chuyển sang trạng thái mới 
        switch (state)
        {
            case State.Moving:
                EnterMoveingState();
                break;
            case State.KnockBack:
                EnterKnockBackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }
    // kiểm tra đối tượng có trong vùng sát thương không 
    private void CheckTouchDamage()
    {
        if(Time.time>= touchDamageCooldown + lastTouchDamageTime)
        {
            TouchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));
            TouchDamageBottLeft.Set(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));

            Collider2D hit = Physics2D.OverlapArea(TouchDamageBottLeft, TouchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                //attackDetails.damageAmount = touchDamage;
                //attackDetails.position.x = alive.transform.position.x;
                //hit.SendMessage("Damage", attackDetails);
            }
        }
    }
    private void OnDrawGizmos()
    {
        // vẽ đường thằng từ groundcheck xuống dưới và từ wallcheck sang bên phải 
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - GroundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        // Vẽ hình chữ nhật 
        Vector2 bottLeft=new Vector2(touchDamageCheck.position.x -(touchDamageWidth/2),touchDamageCheck.position.y-(touchDamageHeight/2));
        Vector2 bottRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));

        Gizmos.DrawLine(bottLeft, bottRight);
        Gizmos.DrawLine(bottRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottLeft);


    }


}
