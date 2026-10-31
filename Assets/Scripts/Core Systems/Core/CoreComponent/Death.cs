using UnityEngine;

public class Death : CoreComponent
{
    [SerializeField] private GameObject[] deathParticles;

    private ParticleManager ParticleManager =>
        particManager ? particManager : core.GetCoreComponent(ref particManager);

    private ParticleManager particManager;
    private States Stats => stats ? stats : core.GetCoreComponent(ref stats);
    private States stats;
    private GameObject healthBarObject;
    private HealthBar healthBarGO;

    public void Die()
    {
        healthBarObject = GameObject.Find("HealthBarContainer");
        healthBarGO = healthBarObject?.GetComponent<HealthBar>();

        foreach (var particle in deathParticles)
        {
            if (ParticleManager != null && particle != null)
            {
                ParticleManager.StartParticles(particle);
            }

            if (gameObject.CompareTag("Player") && healthBarGO != null)
            {
                healthBarGO.slider.value = 0;
                healthBarGO.healthCounter.text = "0/100";
            }
        }

        // Kiểm tra đối tượng là Player hay Enemy
        if (gameObject.CompareTag("Player"))
        {
            if (core != null && core.transform.parent != null)
            {
                core.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            // Nếu là Enemy: Gọi DeadState thông qua Entity StateMachine
            if (core != null && core.transform.parent != null)
            {
                Entity entity = core.transform.parent.GetComponent<Entity>();
                if (entity != null && entity.deadState != null && entity.stateMachine != null)
                {
                    entity.stateMachine.ChangeState(entity.deadState);
                }
                else
                {
                    core.transform.parent.gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        //if (SoundManager.Instance != null && SoundManager.Instance.deathSound != null)
        //{
        //    SoundManager.Instance.PlaySound(SoundManager.Instance.deathSound);
        //}
    }
    private void OnEnable()
    {
        Stats.OnHealthZero += Die;
    }

    private void OnDisable()
    {
        Stats.OnHealthZero -= Die;
    }
}