    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public class AggressiveWeapon : Weapon
    {
        protected SO_AggressiveWeaponData aggressiveWeaponData;
        private List<IDamageable> detectedDamageables = new List<IDamageable>();
        private List<IKnockBack> detectedKnockBackables = new List<IKnockBack>();

        protected override void Awake()
        {
            base.Awake();
            if (weaponData.GetType() == typeof(SO_AggressiveWeaponData))
            {
                aggressiveWeaponData = (SO_AggressiveWeaponData)weaponData;
            }
            else
            {
            }
        }

        public override void AnimationActionTrigger()
        {
            base.AnimationActionTrigger();
            CheckMeleeAttack();
        }

        private void CheckMeleeAttack()
        {
            WeaponAttackDetails attackDetails = aggressiveWeaponData.AttackDetails[attackCount];

            foreach (IDamageable item in detectedDamageables.ToList())
            {
                item.Damage(attackDetails.damageAmount);
            }

            foreach (IKnockBack item in detectedKnockBackables.ToList())
            {
                item.KnockBack(attackDetails.knockbackAngle, attackDetails.knockbackForce, Movement.facingDirection);
            }
        }

        public void AddToDetected(Collider2D collision)
        {
            // Bỏ qua va chạm mặt đất / tường
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>() ?? collision.GetComponentInParent<IDamageable>();
            IKnockBack knockbackable = collision.GetComponent<IKnockBack>() ?? collision.GetComponentInParent<IKnockBack>();

            if (damageable != null)
            {
                if (!detectedDamageables.Contains(damageable))
                {
                    detectedDamageables.Add(damageable);
                }
            }

            if (knockbackable != null)
            {
                if (!detectedKnockBackables.Contains(knockbackable))
                {
                    detectedKnockBackables.Add(knockbackable);
                }
            }
        }

        public void RemoveFromDetected(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                return;
            }

            IDamageable damageable = collision.GetComponent<IDamageable>() ?? collision.GetComponentInParent<IDamageable>();
            IKnockBack knockbackable = collision.GetComponent<IKnockBack>() ?? collision.GetComponentInParent<IKnockBack>();

            if (damageable != null)
            {
                detectedDamageables.Remove(damageable);
            }

            if (knockbackable != null)
            {
                detectedKnockBackables.Remove(knockbackable);
            }
        }
    }