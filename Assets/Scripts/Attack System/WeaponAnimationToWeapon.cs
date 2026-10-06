using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimationToWeapon : MonoBehaviour
{
    private Weapon weapon;

    private void Start()
    {
        weapon = GetComponentInParent<Weapon>();
    }
    private void AnimationFinishTrigger()
    {
        weapon.AnimationFinishedTrigger();

    }
    private void AnimationStartMovementTrigger()
    {
        weapon.AnimationStartMovementTrigger();
    }
    private void AnimationStopMovementTrigger()
    {
        weapon.AnimationStopMovementTrigger();
    }

    private void AnimationCheckOffFlip()
    {
        weapon.AnimationCheckOffFlip();
    }
    private void AnimationCheckOnFlip()
    {
        weapon.AnimationCheckOnFlip();
    }
    private void AnimationActionTrigger()
    {
        weapon.AnimationActionTrigger();
    }
}
