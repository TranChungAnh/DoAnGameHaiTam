using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    //[SerializeField]
    //private bool combatEnable;
    //[SerializeField]
    //private float inputTimer;
    //[SerializeField]
    //private float stunDamageAmount = 1f;
    //private bool gotInput, isAttacking, isFirstAttack;
    //private float lastInputTime = Mathf.NegativeInfinity;
    //[SerializeField]
    //private float  attackRadius, attack1Damage;
    //[SerializeField]
    //private Transform attack1HisBoxPos;
    //[SerializeField]
    //private LayerMask whatisDamageable;

    //private AttackDetails attackDetails;

    //private Animator anim;
    //private PlayerController PC;
    //private PlayerDie PD;
    //private void Start()
    //{
    //    anim = GetComponent<Animator>();
    //    anim.SetBool("canAttack", combatEnable);
    //    PC=GetComponent<PlayerController>();
    //    PD = GetComponent<PlayerDie>();
    //    attackDetails = new AttackDetails();
    //}
    //private void Update()
    //{
    //    CheckCombatInput();
    //    CheckAttacked();
    //}
    //private void CheckCombatInput()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        if (combatEnable)
    //        {
    //            gotInput = true;// đã bấm tấn công 
    //            lastInputTime = Time.time;
    //        }
    //    }
    //}
    //private void CheckAttacked()
    //{
    //    if (gotInput)
    //    {
    //        if (!isAttacking)
    //        {
                
    //            isAttacking = true;
    //            isFirstAttack = !isFirstAttack;
    //            gotInput = false;
    //            anim.SetBool("attack1", true);
    //            anim.SetBool("isAttacking" , isAttacking);
    //            anim.SetBool("FirstAttack", isFirstAttack);
    //        }
    //    }
    //    if(Time.time>=lastInputTime+ inputTimer)
    //    {
    //        gotInput = false;
    //    }
    //}
    //public void CheckAttackHitBox()
    //{
    //    Collider2D[] deletedObjects = Physics2D.OverlapCircleAll(attack1HisBoxPos.position, attackRadius, whatisDamageable);
    //    attackDetails.damageAmount = attack1Damage;
    //    attackDetails.position.x = transform.position.x;
    //    attackDetails.stunDamageAmount = stunDamageAmount;
    //    foreach (Collider2D collider in deletedObjects)
    //    {
    //        collider.transform.parent.SendMessage("Damage1", attackDetails);
    //    }
    //}
    //public void Damage1(AttackDetails attackDetails)
    //{
    //    if (!PC.GetDashStatus())
    //    {
    //        int direction;
    //        if (attackDetails.position.x < transform.position.x)
    //        {
    //            direction = 1;
    //        }
    //        else
    //        {
    //            direction = -1;
    //        }
            
    //        PC.KnockBack(direction);
    //        PD.DecreaseHealth(attackDetails.damageAmount);
    //    }

    //}



    //public void FinishAttack()
    //{
    //    isAttacking = false;
    //    anim.SetBool("FirstAttack", isFirstAttack);
    //    anim.SetBool("attack1", false);

    //}
    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(attack1HisBoxPos.position, attackRadius);
    //}

}
