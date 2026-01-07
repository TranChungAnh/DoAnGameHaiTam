using GameEnums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public float speed = 2;
    public NPCSchedule schedule;

    private Vector3 movetarget;
    private Vector3 lastpos;

    public Animator animator;

    private void Start()
    {
        TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
        lastpos = transform.position;

        // NPC bắt đầu Idle
        movetarget = transform.position;
    }

    private void Update()
    {
        MoveUpdate();
    }

    private void OnMinuteChanged(int hour, int minute)
    {
        var s = schedule.GetSchedule(hour, minute);
        if (s == null) return;

        Vector3 target = schedule.GetRandomPoint(s);

        if (s.stateType == NPCStateType.Move)
        {
            SetDestination(target);
        }
        else
        {
            SetDestination(transform.position); // Idle
        }
    }

    public void SetDestination(Vector3 target)
    {
        movetarget = target;
    }

    private void MoveUpdate()
    {
        Vector3 oldPos = transform.position;

        if (Vector3.Distance(transform.position, movetarget) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                movetarget,
                speed * Time.deltaTime
            );

            // Animation 4 hướng
            Vector3 dir = (transform.position - oldPos).normalized;
            bool isMoving = dir.sqrMagnitude > 0.01f;

            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isIdle", !isMoving);
            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
        }
        else
        {
            // Idle
            animator.SetBool("isMoving", false);
            animator.SetBool("isIdle", true);
        }
    }
}
