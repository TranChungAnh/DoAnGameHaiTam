using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDummyController : MonoBehaviour
{
    [SerializeField]
    private float maxHealth, knockBackSpeedX,KnockBackSpeedY;
    [SerializeField]
    private float knockBackDuration;// khoảng thời gian đẩy lùi 
    [SerializeField]
    private float knockBackDeathSpeedX, knockBackDeathSpeedY, deathTorque;// momen xoắn làm nhân vật quay khi chết 
    [SerializeField]
    private bool applyKnockback;// ấp dụng đẩy lùi 
    [SerializeField]
    private GameObject HitParticle;// hạt va chạm 
    private PlayerController pc;
    private float currenHealth, knockBackStart;
    private GameObject AliveGO, brokenTopGO, brokenBottGO;
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBottom;
    private Animator Aliveanim;
    private int playerFacingDirection;// hướng người chơi 
    private bool playerOnleft;// người chơi đánh sang trái 
    private bool knockBack;

    private void Start()
    {
        currenHealth = maxHealth;
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        AliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("BrokenTop").gameObject;
        brokenBottGO = transform.Find("BrokenBottom").gameObject;

        Aliveanim=AliveGO.GetComponent<Animator>();
        rbAlive = AliveGO.GetComponent<Rigidbody2D>();
        rbBrokenBottom = brokenBottGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();

        AliveGO.SetActive(true);
        brokenTopGO.SetActive(false);
        brokenBottGO.SetActive(false);
    }
    private void Update()
    {
        CheckKnockback();
    }
    //private void Damage1(AttackDetails details)
    //{
    //    currenHealth -= details.damageAmount;
    //    if (details.position.x < AliveGO.transform.position.x)
    //    {
    //        playerFacingDirection = 1;
    //    }
    //    else
    //    {
    //        playerFacingDirection = -1;
    //    }
    //    playerFacingDirection = pc.GetFacingDirection();
    //    // tạo ra hiệu ứng máu bắn 
    //    Instantiate(HitParticle, Aliveanim.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

    //    if (playerFacingDirection == 1)
    //    {
    //        playerOnleft = true;
    //    }
    //    else
    //    {
    //        playerOnleft = false;
    //    }
    //    Aliveanim.SetBool("PlayerOnLeft", playerOnleft);
    //    Aliveanim.SetTrigger("damage");

    //    if(applyKnockback && currenHealth > 0.0f)
    //    {
    //        // đẩy lùi 
    //        KnockBack();
    //    }
    //    if (currenHealth <= 0.0f)
    //    {
    //        //chết 
    //        Die();
    //    }
    //}
    // hiệu ứng đẩy lùi 
    private void KnockBack()
    {
        knockBack = true;
        knockBackStart = Time.time;
        rbAlive.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, KnockBackSpeedY);

    }
    private void CheckKnockback()
    {
        // nếu thời gian hiện tại > thuơif gian đẩy lùi 
        if(Time.time>=knockBackStart+ knockBackDuration && knockBack )
        {
            knockBack = false;
            rbAlive.velocity = new Vector2(0.0f, rbAlive.velocity.y);// 0f để dừng chuyển động của trục x 
        }
    }

    private void Die()
    {
        AliveGO.SetActive(false);
        brokenBottGO.SetActive(true);
        brokenTopGO.SetActive(true);

        brokenTopGO.transform.position = AliveGO.transform.position;
        brokenBottGO.transform.position = AliveGO.transform.position;

        rbBrokenBottom.velocity = new Vector2(knockBackSpeedX * playerFacingDirection, KnockBackSpeedY);
        rbBrokenTop.velocity = new Vector2(knockBackDeathSpeedX * playerFacingDirection, knockBackDeathSpeedY);
        rbBrokenTop.AddTorque(deathTorque * -playerFacingDirection, ForceMode2D.Impulse);
    }
}
