using System.Collections;
using UnityEngine;

public class Combat : CoreComponent, IDamageable, IKnockBack
{
    [SerializeField] private GameObject damageParticles;
    [SerializeField] private float knockBackMaxTime = 0.2f;

    protected Movement Movement { get => movement ?? Core.GetCoreComponent(ref movement); }
    private ParticleManager ParticleManager => particleManager ? particleManager : Core.GetCoreComponent(ref particleManager);
    private States States { get => states ?? Core.GetCoreComponent(ref states); }

    private Movement movement;
    private ParticleManager particleManager;
    private States states;

    // Biến lưu Coroutine hiện tại để quản lý ngắt
    private Coroutine knockBackCoroutine;

    public void Damage(float amount)
    {
        States?.DecreaseHealth(amount);
        ParticleManager?.StartPaticlesWithRandomRotation(damageParticles);
    }

    public void KnockBack(Vector2 angle, float knockbackForce, int direction)
    {
        // Nếu đang có một lượt Knockback chưa chạy xong -> Ngắt ngay Coroutine cũ
        if (knockBackCoroutine != null)
        {
            StopCoroutine(knockBackCoroutine);
        }

        // Bắt đầu lượt Knockback mới
        knockBackCoroutine = StartCoroutine(KnockBackRoutine(knockbackForce, direction));
    }

    private IEnumerator KnockBackRoutine(float knockbackForce, int direction)
    {
        if (Movement != null)
        {
            Movement.canSetVelocity = false; // 1. Khóa di chuyển
            Movement.SetVelocityX(knockbackForce * direction); // 2. Đẩy lùi
        }

        // 3. Đếm ngược đúng knockBackMaxTime giây
        yield return new WaitForSeconds(knockBackMaxTime);

        if (Movement != null)
        {
            Movement.canSetVelocity = true; // 4. Mở lại di chuyển
            Movement.SetVelocityX(0);       // 5. Dừng trượt
        }

        knockBackCoroutine = null; // Reset biến theo dõi
    }
}