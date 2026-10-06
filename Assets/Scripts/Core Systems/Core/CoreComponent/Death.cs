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
            ParticleManager.StartParticles(particle);
            if (gameObject.CompareTag("Player"))
            {
                healthBarGO.slider.value = 0;
                healthBarGO.healthCounter.text = "0/100";
            }
           

        }


        //GameObject entity = core.transform.parent.gameObject;    
        //entity.SetActive(false);
        // Nếu là Player thì ẩn ngay khi chết, nếu là Enemy thì nhường cho DeadState ẩn sau
        if (gameObject.CompareTag("Player"))
        {
            GameObject entity = core.transform.parent.gameObject;
            entity.SetActive(false);
        }
        SoundManager.Instance.PlaySound(SoundManager.Instance.deathSound);
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
